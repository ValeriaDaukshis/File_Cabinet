using System;

namespace FileCabinetApp.Validators.Custom
{
    public class DateOfBirthValidator : IRecordValidator
    {
        private DateTime minDateOfBirth;
        private DateTime maxDateOfBirth;

        public DateOfBirthValidator(DateTime minDateOfBirth, DateTime maxDateOfBirth)
        {
            this.minDateOfBirth = minDateOfBirth;
            this.maxDateOfBirth = maxDateOfBirth;
        }

        public void ValidateParameters(FileCabinetRecord record)
        {
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