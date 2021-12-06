using Hashgraph;

namespace create_token_test_environment
{
    /// <summary>
    /// Contains the details of a token create request and results.
    /// </summary>
    public record TokenInfo
    {
        /// <summary>
        /// The input parameters originally sent to the hedera network.
        /// </summary>
        public CreateTokenParams Params { get; init; }
        /// <summary>
        /// The results received from the hedera network, will include 
        /// the token id within the value.
        /// </summary>
        public CreateTokenReceipt Receipt { get; init; }
    }
}
