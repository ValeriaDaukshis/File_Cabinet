using System;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// DefaultValidator.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Validators.IRecordValidator" />
    public class DefaultValidator : IRecordValidator
    {
        public void ValidateParameters(FileCabinetRecord record)
        {
            new DefaultFirstNameValidator().ValidateParameters(record);
            new DefaultLastNameValidator().ValidateParameters(record);
            new DefaultGenderValidator().ValidateParameters(record);
            new DefaultDateOfBirthValidator().ValidateParameters(record);
            new DefaultCreditSumValidator().ValidateParameters(record);
            new DefaultDurationValidator().ValidateParameters(record);
        }
    }
}