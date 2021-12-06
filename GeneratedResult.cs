namespace create_token_test_environment
{
    /// <summary>
    /// Holds the results of a generated scenario.
    /// </summary>
    public class GeneratedResult
    {
        /// <summary>
        /// Information regarding the details of the newly created token.
        /// </summary>

        public TokenInfo Token { get; set; }
        /// <summary>
        /// Information regarding the details of the newly treasury account.
        /// </summary>
        public AccountInfo TreasuryAccount { get; set; }
        /// <summary>
        /// Information regarding the details of the newly created account 
        /// that is suggested to pay the distribution transfer fees.
        /// </summary>
        public AccountInfo DistributionPayerAccount { get; set; }
        /// <summary>
        /// Information regarding the details of the newly created account 
        /// that is suggested to pay for scheduling the distribution transactions.
        /// </summary>
        public AccountInfo SchedulingPayerAccount { get; set; }
        /// <summary>
        /// Array of account information regarding the simulated 
        /// recipient accounts.
        /// </summary>
        public AccountInfo[] RecipientAccounts { get; set; }
        /// <summary>
        /// A list of suggested distribution amounts for each of the 
        /// recipient accounts.
        /// </summary>
        public double[] RecipientAmounts { get; set; }
    }
}
