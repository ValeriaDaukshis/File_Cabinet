using System;
using FileCabinetApp.Records;

namespace FileCabinetApp.Validators.CommonValidators
{
    /// <summary>
    /// LastNameValidator.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Validators.IRecordValidator" />
    public class LastNameValidator : IRecordValidator
    {
        private readonly int minLength;
        private readonly int maxLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="LastNameValidator"/> class.
        /// </summary>
        /// <param name="minLength">The minimum length.</param>
        /// <param name="maxLength">The maximum length.</param>
        public LastNameValidator(int minLength, int maxLength)
        {
            this.minLength = minLength;
            this.maxLength = maxLength;
        }

        /// <summary>
        /// Validates the parameters.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <exception cref="ArgumentException">
        /// lastName
        /// or
        /// lastName.
        /// </exception>
        public void ValidateParameters(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), $"{nameof(record)} is null");
            }

            string lastName = record.LastName;

            if (string.IsNullOrEmpty(lastName))
            {
                throw new ArgumentException(nameof(lastName), $"Id #{record.Id}: Last name is null ({nameof(lastName)})");
            }

            if (lastName.Length < this.minLength || lastName.Length > this.maxLength)
            {
                throw new ArgumentException($"Id #{record.Id}: Last name length length is upper than {this.maxLength} or under than {this.minLength} symbols ({nameof(lastName)})");
            }
        }
    }
}