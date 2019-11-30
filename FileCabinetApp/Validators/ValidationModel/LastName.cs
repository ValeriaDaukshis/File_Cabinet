using System.Text.Json.Serialization;

namespace FileCabinetApp.Validators.ValidationParameters
{
    /// <summary>
    /// LastNameCriterions.
    /// </summary>
    public class LastName
    {
        /// <summary>
        /// Gets or sets the minimum.
        /// </summary>
        /// <value>
        /// The minimum.
        /// </value>
        public int Min { get; set; }

        /// <summary>
        /// Gets or sets the maximum.
        /// </summary>
        /// <value>
        /// The maximum.
        /// </value>
        public int Max { get; set; }
    }
}