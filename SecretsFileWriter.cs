using Hashgraph;
using System;
using System.IO;
using System.Threading.Tasks;

namespace create_token_test_environment
{
    /// <summary>
    /// Orchestrates the creation of the secretes output file.
    /// </summary>
    /// <remarks>
    /// The secrets output file contains a full list of files 
    /// created by the scenario creator, their ID’s, Role and
    /// public and private keys.
    /// </remarks>
    public class SecretsFileWriter : IDisposable
    {
        /// <summary>
        /// Output file’s stream writer.
        /// </summary>
        private StreamWriter _stream;
        /// <summary>
        /// Wrapper around the Stream Writer to format output 
        /// as valid CSV information.
        /// </summary>
        private CsvWriter _writer;
        /// <summary>
        /// Opens the secrets file for writing (overwrites pre-existing files) 
        /// and pre-populates with header information, returning a reference to 
        /// the file writer for further additions of recipient secrets information. 
        /// </summary>
        /// <param name="filePath">
        /// Path to the secrets file to create.  If a file exists at this path 
        /// it will be deleted prior to creating the new file.
        /// </param>
        /// <param name="tokenInfo">
        /// Token information, including the id and decimal values.
        /// </param>
        /// <param name="treasury">
        /// The token’s treasury account information.
        /// </param>
        /// <param name="distributionPayer">
        /// Account information for a potential distribution payer account, 
        /// this account typically pays the actual distribution transfer, it is 
        /// designated as the “scheduled transaction payer” when scheduling payers.  
        /// This is a different account from the scheduling payer.
        /// </param>
        /// <param name="schedulingPayer">
        /// Account information for a potential scheduling payer account (operator), 
        /// this account typically pays for scheduling the distribution transfer, 
        /// not the transfer itself.  This is a different account from the 
        /// distribution payer.
        /// </param>
        public static async Task<SecretsFileWriter> OpenSecretsFile(string filePath, TokenInfo tokenInfo, AccountInfo treasury, AccountInfo distributionPayer, AccountInfo schedulingPayer)
        {
            var secretsWriter = new SecretsFileWriter(filePath);
            await secretsWriter.WriteHeaderInfo(tokenInfo, treasury, distributionPayer, schedulingPayer);
            return secretsWriter;
        }
        /// <summary>
        /// Private constructor that sets up the internal state of the writer.  
        /// Since writing the header is an async function, it is necessary to 
        /// create the OpenSecretsFile method to combine the construction of the 
        /// object paired with writing the initial header  information.
        /// </summary>
        /// <param name="path">
        /// Full pathname of the secrets file to create.
        /// </param>
        private SecretsFileWriter(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            _stream = File.CreateText(path);
            _writer = new CsvWriter(_stream);
        }
        /// <summary>
        /// Incrementally writes the details of a newly created distribution 
        /// recipient to the output file.
        /// </summary>
        /// <param name="acountInfo">
        /// The account info with the details of the recipient account, 
        /// including its id and signing key values.
        /// </param>
        public async Task WriteRecipientInfo(AccountInfo acountInfo)
        {
            await WriteAccountInfo(acountInfo, "Recipient");
        }
        /// <summary>
        /// Internal helper method that writes the header information to a 
        /// secrets file.  The secrets file contains the information 
        /// enumerating the token details, the treasury, payer and recipient 
        /// account information, including ids, public and private keys.
        /// </summary>
        /// <param name="tokenInfo">
        /// Token information, including the id and decimal values.
        /// </param>
        /// <param name="treasury">
        /// The token’s treasury account information.
        /// </param>
        /// <param name="distributionPayer">
        /// Account information for a potential distribution payer account, 
        /// this account typically pays the actual distribution transfer, it is 
        /// designated as the “scheduled transaction payer” when scheduling payers.  
        /// This is a different account from the scheduling payer.
        /// </param>
        /// <param name="schedulingPayer">
        /// Account information for a potential scheduling payer account (operator), 
        /// this account typically pays for scheduling the distribution transfer, 
        /// not the transfer itself.  This is a different account from the 
        /// distribution payer.
        /// </param>
        private async Task WriteHeaderInfo(TokenInfo tokenInfo, AccountInfo treasury, AccountInfo distributionPayer, AccountInfo schedulingPayer)
        {
            await _writer.WriteLineAsync("Token", tokenInfo.Receipt.Token.AsString());
            await _writer.WriteLineAsync("Symbol", tokenInfo.Params.Symbol);
            await _writer.WriteLineAsync("Name", tokenInfo.Params.Name);
            await _writer.WriteLineAsync("Memo", tokenInfo.Params.Memo);
            await _writer.WriteLineAsync("Circulation", tokenInfo.Params.Circulation.ToString());
            await _writer.WriteLineAsync("Decimals", tokenInfo.Params.Decimals.ToString());
            await _writer.WriteLineAsync("");
            await _writer.WriteLineAsync("Type", "Account", "hBar Balance", "Req'd. Sig", "Public Key", "Private Key");
            await WriteAccountInfo(treasury, "Treasury");
            await WriteAccountInfo(distributionPayer, "Dist Payer");
            await WriteAccountInfo(schedulingPayer, "Sched Payer");
            await _stream.FlushAsync();
        }
        /// <summary>
        /// Internal helper method that writes the details for an account in 
        /// a standard format for all accounts.
        /// </summary>
        /// <param name="info">
        /// The account’s information, including ID, private and public keys.
        /// </param>
        /// <param name="type">
        /// The role this account plays in the scenario, such as Treasury, 
        /// Distribution Payer, Scheduling Payer and Recipient.
        /// </param>
        private async Task WriteAccountInfo(AccountInfo info, string type)
        {
            var address = info.Receipt.Address.AsString();
            var balance = info.Params.InitialBalance.ToString();
            var requiredSig = info.Params.Endorsement.RequiredCount.ToString();
            var publicKey = Hex.FromBytes(info.PublicKeys[0]);
            var privateKey = Hex.FromBytes(info.PrivateKeys[0]);
            await _writer.WriteLineAsync(type, address, balance, requiredSig, publicKey, privateKey);
            if (info.PublicKeys.Length > 1)
            {
                for (int i = 1; i < info.PublicKeys.Length; i++)
                {
                    publicKey = Hex.FromBytes(info.PublicKeys[i]);
                    privateKey = Hex.FromBytes(info.PrivateKeys[i]);
                    await _writer.WriteLineAsync("", "", "", "", publicKey, privateKey);
                }
            }
            await _stream.FlushAsync();
        }
        /// <summary>
        /// Dispose function that closes the output file and 
        /// releases system resources.
        /// </summary>
        public void Dispose()
        {
            _stream?.Flush();
            _stream?.Dispose();
            _stream = null;
            _writer = null;
        }
    }
}
