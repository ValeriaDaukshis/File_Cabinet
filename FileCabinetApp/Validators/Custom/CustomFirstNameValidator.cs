using System;

namespace FileCabinetApp.Validators.Custom
{
    public class CustomFirstNameValidator : IRecordValidator
    {
        public void ValidateParameters(FileCabinetRecord record)
        {
            string firstName = record.FirstName;

            if (string.IsNullOrEmpty(firstName))
            {
                throw new ArgumentNullException(nameof(firstName), $"{nameof(firstName)}: First name is null");
            }

            if (firstName.Length < 4 || firstName.Length > 60)
            {
                throw new ArgumentException(nameof(firstName), $"{nameof(firstName)}: First name length is upper than 60 or under than 4 symbols");
            }
        }
    }
}