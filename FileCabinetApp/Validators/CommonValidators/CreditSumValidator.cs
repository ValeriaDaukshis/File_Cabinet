using System;

namespace FileCabinetApp.Validators.Custom
{
    public class CreditSumValidator : IRecordValidator
    {
        private decimal minCreditSum;
        private decimal maxCreditSum;

        public CreditSumValidator(decimal minCreditSum, decimal maxCreditSum)
        {
            this.minCreditSum = minCreditSum;
            this.maxCreditSum = maxCreditSum;
        }

        public void ValidateParameters(FileCabinetRecord record)
        {
            decimal creditSum = record.CreditSum;
            if (creditSum > this.maxCreditSum || creditSum < this.minCreditSum)
            {
                throw new ArgumentException($"{nameof(creditSum)}: Credit sum is upper than {this.maxCreditSum} or under than {this.minCreditSum} BYN");
            }
        }
    }
}