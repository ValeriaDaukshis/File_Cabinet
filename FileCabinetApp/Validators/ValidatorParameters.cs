using System;

namespace FileCabinetApp.Validators
{
    public class ValidatorParameters
    {
        public decimal minCreditSum { get; private set; }

        public decimal maxCreditSum { get; private set; }

        public int minLength { get; private set; }

        public int maxLength { get; private set; }

        public short minPeriod { get; private set; }

        public short maxPeriod { get; private set; }

        public DateTime minDateOfBirth { get; private set; }

        public DateTime maxDateOfBirth { get; private set; }

        public ValidatorParameters(int minLength, int maxLength, DateTime minDateOfBirth, DateTime maxDateOfBirth, decimal minCreditSum, decimal maxCreditSum, short minPeriod, short maxPeriod)
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
    }
}