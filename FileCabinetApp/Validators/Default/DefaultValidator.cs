using System;
using FileCabinetApp.Validators.Custom;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// DefaultValidator.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Validators.IRecordValidator" />
    public class DefaultValidator : CompositeValidator
    {
        public DefaultValidator(int minLength, int maxLength, DateTime minDateOfBirth, DateTime maxDateOfBirth,
            decimal minCreditSum, decimal maxCreditSum, short minPeriod, short maxPeriod)
            : base(new IRecordValidator[]
            {
                new FirstNameValidator(minLength, maxLength),
                new LastNameValidator(minLength, maxLength),
                new GenderValidator(),
                new DateOfBirthValidator(minDateOfBirth, maxDateOfBirth),
                new CreditSumValidator(minCreditSum, maxCreditSum),
                new DurationValidator(minPeriod, maxPeriod),
            })
        {
        }
    }
}