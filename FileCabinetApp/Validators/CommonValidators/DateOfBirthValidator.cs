using System;
using FileCabinetApp.Records;

namespace FileCabinetApp.Validators.CommonValidators
{
    /// <summary>
    /// DateOfBirthValidator.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Validators.IRecordValidator" />
    public class DateOfBirthValidator : IRecordValidator
    {
        private readonly DateTime minDateOfBirth;
        private readonly DateTime maxDateOfBirth;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateOfBirthValidator"/> class.
        /// </summary>
        /// <param name="minDateOfBirth">The minimum date of birth.</param>
        /// <param name="maxDateOfBirth">The maximum date of birth.</param>
        public DateOfBirthValidator(DateTime minDateOfBirth, DateTime maxDateOfBirth)
        {
            this.minDateOfBirth = minDateOfBirth;
            this.maxDateOfBirth = maxDateOfBirth;
        }

        /// <summary>
        /// Validates the parameters.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <exception cref="ArgumentException">
        /// dateOfBirth
        /// or
        /// dateOfBirth.
        /// </exception>
        public void ValidateParameters(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), $"{nameof(record)} is null");
            }

            DateTime dateOfBirth = record.DateOfBirth;
            if (dateOfBirth > this.maxDateOfBirth)
            {
                throw new ArgumentException(nameof(dateOfBirth), $"{nameof(dateOfBirth)}: Date of birth is upper than {this.maxDateOfBirth}");
            }

            if (dateOfBirth < this.minDateOfBirth)
            {
                throw new ArgumentException(nameof(dateOfBirth), $"{nameof(dateOfBirth)}: Date of birth is under than {this.minDateOfBirth}");
            }
        }
    }
}