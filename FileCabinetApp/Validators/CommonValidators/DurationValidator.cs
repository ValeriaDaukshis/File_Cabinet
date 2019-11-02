using System;
using FileCabinetApp.Records;

namespace FileCabinetApp.Validators.CommonValidators
{
    /// <summary>
    /// DurationValidator.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Validators.IRecordValidator" />
    public class DurationValidator : IRecordValidator
    {
        private readonly short minPeriod;
        private readonly short maxPeriod;

        /// <summary>
        /// Initializes a new instance of the <see cref="DurationValidator"/> class.
        /// </summary>
        /// <param name="minPeriod">The minimum period.</param>
        /// <param name="maxPeriod">The maximum period.</param>
        public DurationValidator(short minPeriod, short maxPeriod)
        {
            this.minPeriod = minPeriod;
            this.maxPeriod = maxPeriod;
        }

        /// <summary>
        /// Validates the parameters.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <exception cref="ArgumentException">duration.</exception>
        public void ValidateParameters(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), $"{nameof(record)} is null");
            }

            short duration = record.Duration;
            if (duration > this.maxPeriod || duration < this.minPeriod)
            {
                throw new ArgumentException(nameof(duration), $"{nameof(duration)}: Credit duration is upper than {this.maxPeriod} or under than {this.minPeriod} weeks");
            }
        }
    }
}