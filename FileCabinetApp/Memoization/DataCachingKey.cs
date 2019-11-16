using System.Collections.Generic;
using System.Linq;

namespace FileCabinetApp.Memoization
{
    /// <summary>
    /// DataCachingKey.
    /// </summary>
    public class DataCachingKey
    {
        private IEnumerable<string> fields;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataCachingKey"/> class.
        /// </summary>
        /// <param name="fields">The fields.</param>
        public DataCachingKey(string[] fields)
        {
            this.Fields = fields.AsEnumerable();
        }

        /// <summary>
        /// Gets or sets the fields.
        /// </summary>
        /// <value>
        /// The fields.
        /// </value>
        public IEnumerable<string> Fields
        {
            get => this.fields;
            set
            {
                if (value != null)
                {
                    this.fields = value;
                }
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((DataCachingKey)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return this.fields != null ? this.fields.GetHashCode() : 0;
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>true if other equals to DataCachingKey.</returns>
        public bool Equals(DataCachingKey other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.fields.SequenceEqual(other.fields);
        }
    }
}