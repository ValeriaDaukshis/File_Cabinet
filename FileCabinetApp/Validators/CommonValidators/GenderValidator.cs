using System;
using FileCabinetApp.Records;

namespace FileCabinetApp.Validators.CommonValidators
{
    /// <summary>
    /// GenderValidator.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Validators.IRecordValidator" />
    public class GenderValidator : IRecordValidator
    {
        /// <summary>
        /// Validates the parameters.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <exception cref="ArgumentException">gender.</exception>
        public void ValidateParameters(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), $"{nameof(record)} is null");
            }

            char gender = record.Gender;
            if (gender != 'M' && gender != 'F')
            {
                throw new ArgumentException($"Id #{record.Id} : indefinite gender ({nameof(gender)}) ");
            }
        }
    }
}