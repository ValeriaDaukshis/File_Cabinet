using System;
using FileCabinetApp.Validators.Custom;

namespace FileCabinetApp.Validators
{
    public class CustomValidator : IRecordValidator
    {
        public void ValidateParameters(FileCabinetRecord record)
        {
            new CustomFirstNameValidator().ValidateParameters(record);
            new CustomLastNameValidator().ValidateParameters(record);
            new CustomGenderValidator().ValidateParameters(record);
            new CustomDateOfBirthValidator().ValidateParameters(record);
            new CustomCreditSumValidator().ValidateParameters(record);
            new CustomDurationValidator().ValidateParameters(record);
        }
    }
}