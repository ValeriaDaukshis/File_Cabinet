using System;

namespace FileCabinetApp.Validators.Custom
{
    public class CustomGenderValidator : IRecordValidator
    {
        public void ValidateParameters(FileCabinetRecord record)
        {
            char gender = record.Gender;
            if (gender != 'M' && gender != 'F')
            {
                throw new ArgumentException(nameof(gender), $"{nameof(gender)}: Indefinite gender");
            }
        }
    }
}