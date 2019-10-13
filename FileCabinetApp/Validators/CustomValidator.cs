using System;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// CustomValidator.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Validators.IRecordValidator" />
    public class CustomValidator : IRecordValidator
    {
        /// <summary>
        /// Validates the parameters.
        /// </summary>
        /// <param name="record">The record.</param>
        public void ValidateParameters(FileCabinetRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException($"{nameof(record)}: record is null");
            }

            if (string.IsNullOrEmpty(record.FirstName))
            {
                throw new ArgumentNullException($"{nameof(record.FirstName)}: First name is null");
            }

            if (string.IsNullOrEmpty(record.LastName))
            {
                throw new ArgumentNullException($"{nameof(record.LastName)}: Last name is null");
            }

            if (record.FirstName.Length < 4 || record.FirstName.Length > 60)
            {
                throw new ArgumentException($"{nameof(record.FirstName)}: First name length is upper than 60 or under than 4 symbols");
            }

            if (record.LastName.Length < 4 || record.LastName.Length > 60)
            {
                throw new ArgumentException($"{nameof(record.LastName)}: Last name length is upper than 60 or under than 4 symbols");
            }

            if (record.Gender != 'M' && record.Gender != 'F')
            {
                throw new ArgumentException($"{nameof(record.Gender)}: Indefinite gender");
            }

            if (record.DateOfBirth > DateTime.Now)
            {
                throw new ArgumentException($"{nameof(record.DateOfBirth)}: Date of birth is upper than today's date");
            }

            if (record.DateOfBirth < new DateTime(1930, 01, 01))
            {
                throw new ArgumentException($"{nameof(record.DateOfBirth)}: Date of birth is under than 01-Jan-1930");
            }

            if (record.CreditSum > 5000 || record.CreditSum < 10)
            {
                throw new ArgumentException($"{nameof(record.CreditSum)}: Credit sum is upper than 5000 or under than 10 BYN");
            }

            if (record.Duration > 500 || record.Duration < 12)
            {
                throw new ArgumentException($"{nameof(record.Duration)}: Credit duration is upper than 500 or under than 12 weeks");
            }
        }
    }
}