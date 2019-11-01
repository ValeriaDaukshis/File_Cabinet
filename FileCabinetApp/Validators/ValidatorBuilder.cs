using System;
using System.Collections.Generic;
using FileCabinetApp.Validators.Custom;

namespace FileCabinetApp.Validators
{
    public class ValidatorBuilder
    {
        private List<IRecordValidator> recordValidators = new List<IRecordValidator>();

        public ValidatorBuilder ValidateFirstName(int minLength, int maxLength)
        {
            this.recordValidators.Add(new FirstNameValidator(minLength, maxLength));
            return this;
        }

        public ValidatorBuilder ValidateLastName(int minLength, int maxLength)
        {
            this.recordValidators.Add(new LastNameValidator(minLength, maxLength));
            return this;
        }

        public ValidatorBuilder ValidateGender()
        {
            this.recordValidators.Add(new GenderValidator());
            return this;
        }

        public ValidatorBuilder ValidateDateOfBirth(DateTime minDateOfBirth, DateTime maxDateOfBirth)
        {
            this.recordValidators.Add(new DateOfBirthValidator(minDateOfBirth, maxDateOfBirth));
            return this;
        }

        public ValidatorBuilder ValidateCreditSum(decimal minCreditSum, decimal maxCreditSum)
        {
            this.recordValidators.Add(new CreditSumValidator(minCreditSum, maxCreditSum));
            return this;
        }

        public ValidatorBuilder ValidateDuration(short minPeriod, short maxPeriod)
        {
            this.recordValidators.Add(new DurationValidator(minPeriod, maxPeriod));
            return this;
        }

        public IRecordValidator Create()
        {
            return new CompositeValidator(this.recordValidators);
        }
    }
}