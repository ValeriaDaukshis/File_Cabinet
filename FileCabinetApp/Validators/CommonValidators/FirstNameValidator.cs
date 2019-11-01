using System;

namespace FileCabinetApp.Validators.Custom
{
    public class FirstNameValidator : IRecordValidator
    {
        private int minLength;
        private int maxLength;

        public FirstNameValidator(int minLength, int maxLength)
        {
            this.minLength = minLength;
            this.maxLength = maxLength;
        }

        public void ValidateParameters(FileCabinetRecord record)
        {
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