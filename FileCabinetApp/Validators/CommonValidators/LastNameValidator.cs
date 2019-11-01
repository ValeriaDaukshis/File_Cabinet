using System;

namespace FileCabinetApp.Validators.Custom
{
    public class LastNameValidator : IRecordValidator
    {
        private int minLength;
        private int maxLength;

        public LastNameValidator(int minLength, int maxLength)
        {
            this.minLength = minLength;
            this.maxLength = maxLength;
        }

        public void ValidateParameters(FileCabinetRecord record)
        {
            string lastName = record.LastName;

            if (string.IsNullOrEmpty(lastName))
            {
                throw new ArgumentException(nameof(lastName), $"{nameof(lastName)}: Last name is null");
            }

            if (lastName.Length < this.minLength || lastName.Length > this.maxLength)
            {
                throw new ArgumentException(nameof(lastName), $"{nameof(lastName)}: Last name length length is upper than {this.maxLength} or under than {this.minLength} symbols");
            }
        }
    }
}