using System;
using FileCabinetApp.Validators.Custom;

namespace FileCabinetApp.Validators
{
    public class CustomValidator : IRecordValidator
    {
        private decimal minCreditSum;
        private decimal maxCreditSum;
        private int minLength;
        private int maxLength;
        private short minPeriod;
        private short maxPeriod;
        private DateTime minDateOfBirth;
        private DateTime maxDateOfBirth;

        public CustomValidator(int minLength, int maxLength, DateTime minDateOfBirth, DateTime maxDateOfBirth, decimal minCreditSum, decimal maxCreditSum, short minPeriod, short maxPeriod)
        {
            this.minLength = minLength;
            this.maxLength = maxLength;
            this.minDateOfBirth = minDateOfBirth;
            this.maxDateOfBirth = maxDateOfBirth;
            this.minCreditSum = minCreditSum;
            this.maxCreditSum = maxCreditSum;
            this.minPeriod = minPeriod;
            this.maxPeriod = maxPeriod;
        }

        public void ValidateParameters(FileCabinetRecord record)
        {
            new FirstNameValidator(this.minLength, this.maxLength).ValidateParameters(record);
            new LastNameValidator(this.minLength, this.maxLength).ValidateParameters(record);
            new GenderValidator().ValidateParameters(record);
            new DateOfBirthValidator(this.minDateOfBirth, this.maxDateOfBirth).ValidateParameters(record);
            new CreditSumValidator(this.minCreditSum, this.maxCreditSum).ValidateParameters(record);
            new DurationValidator(this.minPeriod, this.maxPeriod).ValidateParameters(record);
        }
    }
}