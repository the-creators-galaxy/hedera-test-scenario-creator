using Hashgraph;
using System;
using System.IO;

namespace create_token_test_environment
{
    /// <summary>
    /// Aggregates all user entered scenario builder configuration.
    /// </summary>
    public class InputConfiguration
    {
        /// <summary>
        /// Newly created Token Symbol
        /// </summary>
        public string TokenSymbol { get; set; } = "MYRO";
        /// <summary>
        /// Newly created Token Name
        /// </summary>
        public string TokenName { get; set; } = "Myro's Domain";
        /// <summary>
        /// Newly created Token Memo
        /// </summary>
        public string TokenMemo { get; set; } = "You didn't see anything.";
        /// <summary>
        /// Total circulation of the new token (in smallest denomination).
        /// </summary>
        public ulong TokenCirculation { get; set; } = 100_000_000_00_000_000;
        /// <summary>
        /// Number of decimal places of the token.
        /// </summary>
        public uint TokenDecimalPlaces { get; set; } = 8;
        /// <summary>
        /// The total number of individual keys associated with the treasury account.
        /// </summary>
        public int TreasuryTotalKeyCount { get; set; } = 3;
        /// <summary>
        /// The total number of individual keys required to sign a transfer from the treasury.
        /// </summary>
        public uint TreasuryRequiredSignatureCount { get; set; } = 2;
        /// <summary>
        /// The initial hBar balance the treasury is created with.
        /// </summary>
        public ulong TreasuryInitialCryptoBalance { get; set; } = 100_00_000_000;
        /// <summary>
        /// Initial hBar balance for the suggested distribution payer account.
        /// </summary>
        public ulong DistributionPayerInitialCryptoBalance { get; set; } = 100_00_000_000;
        /// <summary>
        /// Initial hBar balance for the suggested scheduling payer account.
        /// </summary>
        public ulong SchedulingPayerInitialCryptoBalance { get; set; } = 100_00_000_000;
        /// <summary>
        /// The number of recipient accounts to attempt to create.
        /// </summary>
        public long RecipientCount { get; set; } = 20;
        /// <summary>
        /// The initial hBar balance of each recipient account.
        /// </summary>
        public ulong RecipientInitialCryptoBalance { get; set; } = 0;
        /// <summary>
        /// The minimum value of the randomly generated amount for a distribution.
        /// </summary>
        public long MiniumDistribution { get; set; } = 500_000;
        /// <summary>
        /// The maximum value of the randomly generated amount for a distribution.
        /// </summary>
        public long MaximumDistribution { get; set; } = 10_000_00_000_000;
        /// <summary>
        /// The hedera node to send all transactions and queries to.
        /// </summary>
        public Gateway Gateway { get; set; } = new Gateway("34.83.112.116:50211", 0, 0, 6);
        /// <summary>
        /// The payer account address paying for the creation of scenario accounts.
        /// </summary>
        /// <remarks>
        /// The deafult is an example value, it will not work against any network.
        /// </remarks>
        public Address Payer { get; set; } = new Address(0, 0, 1001);
        /// <summary>
        /// The private key for the payer account address paying for the creation 
        /// of scenario accounts, used to sign creation transactions.
        /// </summary>
        /// <remarks>
        /// The default value is the genesis key found in the hedera-services code base, 
        /// it will not work for any deployed network.
        /// </remarks>
        public ReadOnlyMemory<byte> PrivateKey { get; set; } = Hex.ToBytes("302e020100300506032b65700422042091132178e72057a1d7528025956fe39b0b847f200ab59b2fdd367017f3087137");
        /// <summary>
        /// The full path of the distribution csv file created by the tool.  
        /// It will contain the list of created accounts receiving distributions 
        /// and a random amount assigned to each receiver.  The file format is 
        /// compatible with the token distribution tool without modification.
        /// </summary>
        public string OutputCsvFile { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "test-distribution.csv");
        /// <summary>
        /// The full path of the secrets file containing a listing of all scenario 
        /// participant accounts created by this tool.  It includes the public and 
        /// private keys for each account in addition to their account ids.
        /// </summary>
        public string OutputSecretsFile { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "test-distribution-secrets.csv");
    }
}
