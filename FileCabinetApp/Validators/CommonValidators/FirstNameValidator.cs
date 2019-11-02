using System;
using FileCabinetApp.Records;

namespace FileCabinetApp.Validators.CommonValidators
{
    /// <summary>
    /// FirstNameValidator.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Validators.IRecordValidator" />
    public class FirstNameValidator : IRecordValidator
    {
        private readonly int minLength;
        private readonly int maxLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirstNameValidator"/> class.
        /// </summary>
        /// <param name="minLength">The minimum length.</param>
        /// <param name="maxLength">The maximum length.</param>
        public FirstNameValidator(int minLength, int maxLength)
        {
            this.minLength = minLength;
            this.maxLength = maxLength;
        }

        /// <summary>
        /// Validates the parameters.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <exception cref="ArgumentException">
        /// firstName
        /// or
        /// firstName.
        /// </exception>
        public void ValidateParameters(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), $"{nameof(record)} is null");
            }

            string firstName = record.FirstName;

            if (string.IsNullOrEmpty(firstName))
            {
                throw new ArgumentException(nameof(firstName), $"{nameof(firstName)}: First name is null");
            }

            if (firstName.Length < this.minLength || firstName.Length > this.maxLength)
            {
                throw new ArgumentException(nameof(firstName), $"{nameof(firstName)}: First name length is upper than {this.maxLength} or under than {this.minLength} symbols");
            }
        }
    }
}