using System;

namespace FileCabinetApp.Validators.Custom
{
    public class CustomLastNameValidator : IRecordValidator
    {
        public void ValidateParameters(FileCabinetRecord record)
        {
            string lastName = record.LastName;

            if (string.IsNullOrEmpty(lastName))
            {
                throw new ArgumentNullException(nameof(lastName), $"{nameof(lastName)}: First name is null");
            }

            if (lastName.Length < 4 || lastName.Length > 60)
            {
                throw new ArgumentException(nameof(lastName), $"{nameof(lastName)}: First name length is upper than 60 or under than 4 symbols");
            }
        }
    }
}