using Hashgraph;

namespace create_token_test_environment
{
    /// <summary>
    /// General extension helper methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Helper function to display an address id in shard.realm.num string format.
        /// </summary>
        /// <param name="address">
        /// Address to display.
        /// </param>
        /// <returns>
        /// String representation of the address.
        /// </returns>
        public static string AsString(this Address address)
        {
            return $"{address.ShardNum}.{address.RealmNum}.{address.AccountNum}";
        }
    }
}
