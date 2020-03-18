using System;
using System.Collections.Generic;

using FileCabinetApp.Validators.CommonValidators;

namespace FileCabinetApp.Validators
{
    /// <summary>
    ///     ValidatorBuilder.
    /// </summary>
    public class ValidatorBuilder
    {
        private readonly List<IRecordValidator> recordValidators = new List<IRecordValidator>();

        /// <summary>
        ///     Creates this instance.
        /// </summary>
        /// <returns>IRecordValidator.</returns>
        public IRecordValidator Create()
        {
            return new CompositeValidator(this.recordValidators);
        }

        /// <summary>
        ///     Validates the credit sum.
        /// </summary>
        /// <param name="minCreditSum">The minimum credit sum.</param>
        /// <param name="maxCreditSum">The maximum credit sum.</param>
        /// <returns>ValidatorBuilder.</returns>
        public ValidatorBuilder ValidateCreditSum(decimal minCreditSum, decimal maxCreditSum)
        {
            this.recordValidators.Add(new CreditSumValidator(minCreditSum, maxCreditSum));
            return this;
        }

        /// <summary>
        ///     Validates the date of birth.
        /// </summary>
        /// <param name="minDateOfBirth">The minimum date of birth.</param>
        /// <param name="maxDateOfBirth">The maximum date of birth.</param>
        /// <returns>ValidatorBuilder.</returns>
        public ValidatorBuilder ValidateDateOfBirth(DateTime minDateOfBirth, DateTime maxDateOfBirth)
        {
            this.recordValidators.Add(new DateOfBirthValidator(minDateOfBirth, maxDateOfBirth));
            return this;
        }

        /// <summary>
        ///     Validates the duration.
        /// </summary>
        /// <param name="minPeriod">The minimum period.</param>
        /// <param name="maxPeriod">The maximum period.</param>
        /// <returns>ValidatorBuilder.</returns>
        public ValidatorBuilder ValidateDuration(short minPeriod, short maxPeriod)
        {
            this.recordValidators.Add(new DurationValidator(minPeriod, maxPeriod));
            return this;
        }

        /// <summary>
        ///     Validates the first name.
        /// </summary>
        /// <param name="minLength">The minimum length.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <returns>ValidatorBuilder.</returns>
        public ValidatorBuilder ValidateFirstName(int minLength, int maxLength)
        {
            this.recordValidators.Add(new FirstNameValidator(minLength, maxLength));
            return this;
        }

        /// <summary>
        ///     Validates the gender.
        /// </summary>
        /// <returns>ValidatorBuilder.</returns>
        public ValidatorBuilder ValidateGender()
        {
            this.recordValidators.Add(new GenderValidator());
            return this;
        }

        /// <summary>
        ///     Validates the last name.
        /// </summary>
        /// <param name="minLength">The minimum length.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <returns>ValidatorBuilder.</returns>
        public ValidatorBuilder ValidateLastName(int minLength, int maxLength)
        {
            this.recordValidators.Add(new LastNameValidator(minLength, maxLength));
            return this;
        }
    }
}