using Hashgraph;
using System;

namespace create_token_test_environment
{
    /// <summary>
    /// Holds the information regarding the creation of a hedera account.
    /// </summary>
    public record AccountInfo
    {
        /// <summary>
        /// Array holding the public keys associated with the account.
        /// </summary>
        public ReadOnlyMemory<byte>[] PublicKeys { get; init; }
        /// <summary>
        /// Array holding the private keys associated with the account.
        /// </summary>
        public ReadOnlyMemory<byte>[] PrivateKeys { get; init; }
        /// <summary>
        /// The original creation parameters sent to the network when 
        /// creating this account.
        /// </summary>
        public CreateAccountParams Params { get; init; }
        /// <summary>
        /// The receipt returned from the network when this account 
        /// was created, it includes the address id of the newly 
        /// created account.
        /// </summary>
        public CreateAccountReceipt Receipt { get; init; }
    }
}
