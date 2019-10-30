using System;

namespace FileCabinetApp.Validators
{
    public class DefaultCreditSumValidator : IRecordValidator
    {
        public void ValidateParameters(FileCabinetRecord record)
        {
            decimal creditSum = record.CreditSum;
            if (creditSum > 5000 || creditSum < 10)
            {
                throw new ArgumentException(nameof(creditSum), $"{nameof(creditSum)}: Credit sum is upper than 5000 or under than 10 BYN");
            }
        }
    }
}