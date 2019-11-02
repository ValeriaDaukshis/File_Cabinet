using System;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// ValidatorsExtensions.
    /// </summary>
    public static class ValidatorsExtensions
    {
        /// <summary>
        /// Creates the custom.
        /// </summary>
        /// <param name="validatorBuilder">The validator builder.</param>
        /// <returns> (IRecordValidator, ValidatorParameters).</returns>
        public static (IRecordValidator, ValidatorParameters) CreateCustom(this ValidatorBuilder validatorBuilder)
        {
            if (validatorBuilder is null)
            {
                throw new ArgumentNullException(nameof(validatorBuilder), $"{nameof(validatorBuilder)} is null");
            }

            var validatorParameters = new ValidatorParameters(2, 60, new DateTime(1930, 1, 1), DateTime.Now, 100, 5000, 6, 120);
            return (CreateValidator(validatorParameters, validatorBuilder), validatorParameters);
        }

        /// <summary>
        /// Creates the default.
        /// </summary>
        /// <param name="validatorBuilder">The validator builder.</param>
        /// <returns> (IRecordValidator, ValidatorParameters).</returns>
        public static (IRecordValidator, ValidatorParameters) CreateDefault(this ValidatorBuilder validatorBuilder)
        {
            if (validatorBuilder is null)
            {
                throw new ArgumentNullException(nameof(validatorBuilder), $"{nameof(validatorBuilder)} is null");
            }

            var validatorParameters = new ValidatorParameters(4, 60, new DateTime(1950, 1, 1), DateTime.Now, 100, 10_000, 6, 500);
            return (CreateValidator(validatorParameters, validatorBuilder), validatorParameters);
        }

        private static IRecordValidator CreateValidator(ValidatorParameters validatorParameters, ValidatorBuilder validatorBuilder)
        {
            IRecordValidator recordValidator = validatorBuilder
                .ValidateFirstName(validatorParameters.MinLength, validatorParameters.MaxLength)
                .ValidateLastName(validatorParameters.MinLength, validatorParameters.MaxLength)
                .ValidateGender()
                .ValidateDateOfBirth(validatorParameters.MinDateOfBirth, validatorParameters.MaxDateOfBirth)
                .ValidateCreditSum(validatorParameters.MinCreditSum, validatorParameters.MaxCreditSum)
                .ValidateDuration(validatorParameters.MinPeriod, validatorParameters.MaxPeriod)
                .Create();
            return recordValidator;
        }
    }
}