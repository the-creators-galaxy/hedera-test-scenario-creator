using Hashgraph;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Spectre.Console;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace create_token_test_environment
{
    /// <summary>
    /// Orchestrates the generation of a test scenario using criteria input by the user.
    /// </summary>
    public class Generator
    {
        /// <summary>
        /// Public key prefix for ANS.1 encoding, useful for generating files with interoperable key representations.
        /// </summary>
        internal static readonly byte[] publicKeyPrefix = Hex.ToBytes("302a300506032b6570032100").ToArray();
        /// <summary>
        /// Private key prefix for ANS.1 encoding, useful for generating files with interoperable key representations.
        /// </summary>
        internal static readonly byte[] privateKeyPrefix = Hex.ToBytes("302e020100300506032b657004220420").ToArray();
        /// <summary>
        /// Creates a test scenario adhering to the configuration criteria, including creating a token, 
        /// treasury account, distribution payer account, scheduling payer account and the required 
        /// number of recipient accounts on the target hedera network.
        /// </summary>
        /// <param name="input">
        /// The input criteria entered by the user.
        /// </param>
        /// <returns>
        /// Results object holding information for all of the assets created.
        /// </returns>
        public static async Task<GeneratedResult> CreateTestEnvironment(InputConfiguration input)
        {
            var result = new GeneratedResult();
            var random = new Random();
            var decimalMultiplier = 1.0 / Math.Pow(10, input.TokenDecimalPlaces);
            var minDist = input.MiniumDistribution * decimalMultiplier;
            var maxDist = input.MaximumDistribution * decimalMultiplier;
            var rangeDist = maxDist - minDist;
            await using var client = new Client(cfg =>
            {
                cfg.Gateway = input.Gateway;
                cfg.Payer = input.Payer;
                cfg.Signatory = new Signatory(input.PrivateKey);
            });
            await AnsiConsole.Status()
                .AutoRefresh(true)
                .Spinner(Spinner.Known.Default)
                .StartAsync("[yellow]Creating Treasury Account[/]...", async ctx =>
                {
                    result.TreasuryAccount = await CreateCryptoAccountAsync(client, input.TreasuryTotalKeyCount, input.TreasuryRequiredSignatureCount, input.TreasuryInitialCryptoBalance);
                    AnsiConsole.MarkupLine($"Treasury Account {result.TreasuryAccount.Receipt.Address.AsString()} Created.");
                    ctx.Status("[yellow]Creating Token[/]...");
                    result.Token = await CreateTokenAsync(client, input.TokenSymbol, input.TokenName, input.TokenMemo, input.TokenCirculation, input.TokenDecimalPlaces, result.TreasuryAccount);
                    AnsiConsole.MarkupLine($"Token {input.TokenSymbol} created at Address {result.Token.Receipt.Token.AsString()}.");
                    ctx.Status("[yellow]Creating Distribution Payer Account[/]...");
                    result.DistributionPayerAccount = await CreateCryptoAccountAsync(client, 1, 1, input.DistributionPayerInitialCryptoBalance);
                    AnsiConsole.MarkupLine($"Distribution Payer Account {result.DistributionPayerAccount.Receipt.Address.AsString()} Created.");
                    ctx.Status("[yellow]Creating Scheduling Payer Account[/]...");
                    result.SchedulingPayerAccount = await CreateCryptoAccountAsync(client, 1, 1, input.SchedulingPayerInitialCryptoBalance);
                    AnsiConsole.MarkupLine($"Scheduling Payer Account {result.SchedulingPayerAccount.Receipt.Address.AsString()} Created.");
                    result.RecipientAccounts = new AccountInfo[input.RecipientCount];
                    result.RecipientAmounts = new double[input.RecipientCount];

                    using var secretsWriter = await SecretsFileWriter.OpenSecretsFile(input.OutputSecretsFile, result.Token, result.TreasuryAccount, result.DistributionPayerAccount, result.SchedulingPayerAccount);
                    using var distributionWriter = await DistributionFileWriter.OpenDistributionFile(input.OutputCsvFile);
                    var accountsChannel = Channel.CreateUnbounded<(AccountInfo accountInfo, double amount)>();
                    var accountsWriter = accountsChannel.Writer;
                    var accountsReader = accountsChannel.Reader;
                    _ = Task.Run(async () =>
                    {
                        while (await accountsReader.WaitToReadAsync())
                        {
                            while (accountsReader.TryRead(out (AccountInfo accountInfo, double amount) item))
                            {
                                await secretsWriter.WriteRecipientInfo(item.accountInfo);
                                await distributionWriter.WriteRecipientInfo(item.accountInfo, item.amount);
                            }
                        }
                    });
                    int nextIndex = -1;
                    int accountsCreated = 0;
                    try
                    {
                        await Task.WhenAll(Enumerable.Range(1, 6).Select(index => Task.Run(async () =>
                        {
                            for (var i = Interlocked.Increment(ref nextIndex); i < input.RecipientCount; i = Interlocked.Increment(ref nextIndex))
                            {
                                lock (ctx)
                                {
                                    ctx.Status($"[yellow]Recipient Accounts Created: [white]{accountsCreated}[/] ...[/]");
                                }
                                var accountInfo = await CreateCryptoAccountAsync(client, 1, 1, input.RecipientInitialCryptoBalance);
                                await AssociateCryptoAccountAsync(client, result.Token.Receipt.Token, accountInfo);
                                result.RecipientAccounts[i] = accountInfo;
                                lock (random)
                                {
                                    result.RecipientAmounts[i] = Math.Round(random.NextDouble() * rangeDist + minDist, (int)input.TokenDecimalPlaces);
                                }
                                await accountsWriter.WriteAsync((accountInfo, result.RecipientAmounts[i]));
                                Interlocked.Increment(ref accountsCreated);
                            }
                        })));
                    }
                    catch (Exception ex)
                    {
                        ctx.Status("[red]Creation process terminated with error.[/]");
                        AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything | ExceptionFormats.ShowLinks);
                    }
                    finally
                    {
                        accountsWriter.TryComplete();
                    }
                    AnsiConsole.MarkupLine($"{accountsCreated} Recipient Accounts Created and Associated.");
                    ctx.Status("[yellow]Associating Recipient Accounts with Token[/]...");
                    ctx.Status("[yellow]Done.[/]");
                });
            return result;
        }
        /// <summary>
        /// Internal helper function that submits a token associate transaction to the 
        /// target network for a given recipient account address.
        /// </summary>
        /// <param name="client">
        /// A pre-configured hedera node client object.
        /// </param>
        /// <param name="token">
        /// The address of the token to associate.
        /// </param>
        /// <param name="account">
        /// The account info containing the id and signing key that is to be 
        /// associated with the identified token.
        /// </param>
        private static async Task AssociateCryptoAccountAsync(Client client, Address token, AccountInfo account)
        {
            int retry = 0;
            while (true)
            {
                try
                {
                    await client.AssociateTokenAsync(token, account.Receipt.Address, new Signatory(account.PrivateKeys.Select(p => new Signatory(p)).ToArray()));
                    return;
                }
                catch (PrecheckException) when (retry < 1000)
                {
                    // todo: we should limit the status codes
                    // that will trigger a retry, this criteria
                    // may be too broad.
                    retry++;
                }
                catch (TransactionException tex) when (tex.Status == ResponseCode.TokenAlreadyAssociatedToAccount)
                {
                    return;
                }
            }
        }
        /// <summary>
        /// Internal helper function orchestrating the key generation and account creation 
        /// for a new account with the given specifics.
        /// </summary>
        /// <param name="client">
        /// A pre-configured hedera node client object.
        /// </param>
        /// <param name="keyCount">
        /// Number of keys to generate for this account.
        /// </param>
        /// <param name="requiredKeyCount">
        /// The number of keys that will be required to sign transactions 
        /// for this account (must be less than or equal to the key count).
        /// </param>
        /// <param name="initialBalance">
        /// The initial balance (in tinybar) that this account should be created with.
        /// </param>
        /// <returns>
        /// An account information object containing the private keys, input parameters 
        /// and account creation receipt returned from the network (which in turn 
        /// identifies the address of the newly created account).
        /// </returns>
        private static async Task<AccountInfo> CreateCryptoAccountAsync(Client client, int keyCount, uint requiredKeyCount, ulong initialBalance)
        {
            var (publicKeys, privateKeys) = GenerateEd25519KeyPairs(keyCount);
            var createParams = new CreateAccountParams
            {
                InitialBalance = initialBalance,
                Endorsement = new Endorsement(requiredKeyCount, publicKeys.Select(k => new Endorsement(k)).ToArray())
            };
            int retry = 0;
            while (true)
            {
                try
                {
                    var receipt = await client.CreateAccountAsync(createParams);
                    return new AccountInfo
                    {
                        PublicKeys = publicKeys,
                        PrivateKeys = privateKeys,
                        Params = createParams,
                        Receipt = receipt
                    };
                }
                catch (PrecheckException) when (retry < 1000)
                {
                    // todo: we should limit the status codes
                    // that will trigger a retry, this criteria
                    // may be too broad.
                    retry++;
                }
            }
        }
        /// <summary>
        /// Internal helper function orchestrating the creation of a token 
        /// with a given treasury account.
        /// </summary>
        /// <reamrks>
        /// The token created will be immutable but will contain a mint key that 
        /// matches the treasury.  This can be useful if testing scenarios 
        /// exhausts the treasury of tokens, in which case minting additional 
        /// tokens for testing may be more desirable than creating an entirely 
        /// new scenario.
        /// </reamrks>
        /// <param name="client">
        /// A pre-configured hedera node client object.
        /// </param>
        /// <param name="symbol">
        /// The token symbol to create.
        /// </param>
        /// <param name="name">
        /// Name of the token.
        /// </param>
        /// <param name="memo">
        /// Memo associated with the token.
        /// </param>
        /// <param name="circulation">
        /// The initial minted number of tokens (in the smallest denomination).
        /// </param>
        /// <param name="decimals">
        /// The number of decimals for the token.
        /// </param>
        /// <param name="treasury">
        /// The account information for the account that will become the treasury.  
        /// The private keys of this account will sign the creation transaction.
        /// </param>
        /// <returns>
        /// An object containing the input parameters and resulting receipt for 
        /// token creation, which includes the address ID of the created token.
        /// </returns>
        private static async Task<TokenInfo> CreateTokenAsync(Client client, string symbol, string name, string memo, ulong circulation, uint decimals, AccountInfo treasury)
        {
            var createParams = new CreateTokenParams
            {
                Symbol = symbol,
                Name = name,
                Memo = memo,
                Circulation = circulation,
                Decimals = decimals,
                Expiration = DateTime.UtcNow.AddDays(90),
                Treasury = treasury.Receipt.Address,
                SupplyEndorsement = treasury.Params.Endorsement,
                Signatory = new Signatory(treasury.PrivateKeys.Select(p => new Signatory(p)).ToArray())
            };
            var receipt = await client.CreateTokenAsync(createParams);
            return new TokenInfo
            {
                Params = createParams,
                Receipt = receipt
            };
        }
        /// <summary>
        /// Generates an array of randomly generated Ed25519 private and public key pairs.
        /// </summary>
        /// <param name="keyCount">
        /// The number of key pairs to generate.
        /// </param>
        /// <returns>
        /// An array of randomly generated private keys with a corresponding array of 
        /// public keys.
        /// </returns>
        private static (ReadOnlyMemory<byte>[] publicKeys, ReadOnlyMemory<byte>[] privateKeys) GenerateEd25519KeyPairs(int keyCount)
        {
            var publicKeys = new ReadOnlyMemory<byte>[keyCount];
            var privateKeys = new ReadOnlyMemory<byte>[keyCount];
            var gen = new Ed25519KeyPairGenerator();
            gen.Init(new KeyGenerationParameters(new SecureRandom(), 32));
            for (var i = 0; i < keyCount; i++)
            {
                var keys = gen.GenerateKeyPair();
                publicKeys[i] = publicKeyPrefix.Concat((keys.Public as Ed25519PublicKeyParameters).GetEncoded()).ToArray();
                privateKeys[i] = privateKeyPrefix.Concat((keys.Private as Ed25519PrivateKeyParameters).GetEncoded()).ToArray();
            }
            return (publicKeys, privateKeys);
        }
    }
}
