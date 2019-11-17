using System.Collections.Generic;
using System.Linq;
using FileCabinetApp.Records;

namespace FileCabinetApp.Memoization
{
    /// <summary>
    /// DataCaching.
    /// </summary>
    public class DataCaching
    {
        private readonly Dictionary<CachingKey, IEnumerable<FileCabinetRecord>> cacheDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataCaching"/> class.
        /// </summary>
        public DataCaching()
        {
            this.cacheDictionary = new Dictionary<CachingKey, IEnumerable<FileCabinetRecord>>();
        }

        /// <summary>
        /// Gets the value by key.
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <returns>value by key.</returns>
        public IEnumerable<FileCabinetRecord> GetValueByKey(string[] fields)
        {
            return this.cacheDictionary[this.cacheDictionary.Keys.First(t => t.Equals(new CachingKey(fields)))];
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="records">The records.</param>
        public void Add(CachingKey key, IEnumerable<FileCabinetRecord> records)
        {
            this.cacheDictionary.Add(key, records);
        }

        /// <summary>
        /// Determines whether this instance contains the object.
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <returns>
        ///   <c>true</c> if [contains] [the specified fields]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(string[] fields)
        {
            var isValueInDictionary = this.cacheDictionary.Keys.FirstOrDefault(t => t.Equals(new CachingKey(fields)));

            if (isValueInDictionary == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            this.cacheDictionary.Clear();
        }
    }
}