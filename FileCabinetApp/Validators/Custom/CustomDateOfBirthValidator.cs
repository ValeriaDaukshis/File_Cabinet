using System;

namespace FileCabinetApp.Validators.Custom
{
    public class CustomDateOfBirthValidator : IRecordValidator
    {
        /// <summary>
        /// Validates the date of birth.
        /// </summary>
        /// <param name="dateOfBirth">The date of birth.</param>
        public void ValidateParameters(FileCabinetRecord record)
        {
            DateTime dateOfBirth = record.DateOfBirth;
            if (dateOfBirth > DateTime.Now)
            {
                throw new ArgumentException(nameof(dateOfBirth), $"{nameof(dateOfBirth)}: Date of birth is upper than today's date");
            }

            if (dateOfBirth < new DateTime(1930, 01, 01))
            {
                throw new ArgumentException(nameof(dateOfBirth), $"{nameof(dateOfBirth)}: Date of birth is under than 01-Jan-1950");
            }
        }
    }
}