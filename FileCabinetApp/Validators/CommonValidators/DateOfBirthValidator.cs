using System;

using FileCabinetApp.Records;

namespace FileCabinetApp.Validators.CommonValidators
{
    /// <summary>
    ///     DateOfBirthValidator.
    /// </summary>
    /// <seealso cref="IRecordValidator" />
    public class DateOfBirthValidator : IRecordValidator
    {
        private readonly DateTime maxDateOfBirth;

        private readonly DateTime minDateOfBirth;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DateOfBirthValidator" /> class.
        /// </summary>
        /// <param name="minDateOfBirth">The minimum date of birth.</param>
        /// <param name="maxDateOfBirth">The maximum date of birth.</param>
        public DateOfBirthValidator(DateTime minDateOfBirth, DateTime maxDateOfBirth)
        {
            this.minDateOfBirth = minDateOfBirth;
            this.maxDateOfBirth = maxDateOfBirth;
        }

        /// <summary>
        ///     Validates the parameters.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <exception cref="ArgumentException">
        ///     dateOfBirth
        ///     or
        ///     dateOfBirth.
        /// </exception>
        public void ValidateParameters(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), $"{nameof(record)} is null");
            }

            var dateOfBirth = record.DateOfBirth;
            if (dateOfBirth > this.maxDateOfBirth)
            {
                throw new ArgumentException(nameof(dateOfBirth), $"Id #{record.Id} : Date of birth is upper than {this.maxDateOfBirth} ({nameof(dateOfBirth)})");
            }

            if (dateOfBirth < this.minDateOfBirth)
            {
                throw new ArgumentException($"Id #{record.Id}: Date of birth is under than {this.minDateOfBirth} ( {nameof(dateOfBirth)})");
            }
        }
    }
}