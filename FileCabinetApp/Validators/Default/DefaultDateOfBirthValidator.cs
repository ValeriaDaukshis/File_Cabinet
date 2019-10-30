using System;

namespace FileCabinetApp.Validators
{
    public class DefaultDateOfBirthValidator : IRecordValidator
    {
        public void ValidateParameters(FileCabinetRecord record)
        {
            DateTime dateOfBirth = record.DateOfBirth;
            if (dateOfBirth > DateTime.Now)
            {
                throw new ArgumentException(nameof(dateOfBirth), $"{nameof(dateOfBirth)}: Date of birth is upper than today's date");
            }

            if (dateOfBirth < new DateTime(1950, 01, 01))
            {
                throw new ArgumentException(nameof(dateOfBirth), $"{nameof(dateOfBirth)}: Date of birth is under than 01-Jan-1950");
            }
        }
    }
}