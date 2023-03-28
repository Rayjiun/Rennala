namespace Rennala.Utility
{
    sealed internal class Duplication
    {
        private readonly Dictionary<string, string?> _ = new();

        /// <summary>
        /// Add key to duplication dictionary
        /// </summary>
        /// <param name="key">Key to add</param>
        public void Add(string key)
        {
            if (IsDuplicate(key)) // Can't add duplicated keys in dictionaries
            {
                return;
            }

            _.Add(key, null);
        }

        /// <summary>
        /// Check if it's a duplicate
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>bool</returns>
        public bool IsDuplicate(string key)
        {
            return _.ContainsKey(key);
        }
    }
}
