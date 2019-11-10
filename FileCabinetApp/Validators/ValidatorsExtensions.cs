using System;
using FileCabinetApp.Validators.ValidationParameters;
using Microsoft.Extensions.Configuration;

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
        /// <param name="config">The configuration.</param>
        /// <returns>(IRecordValidator, ValidatorParameters).</returns>
        public static (IRecordValidator, ValidatorParameters) CreateCustom(this ValidatorBuilder validatorBuilder, IConfigurationRoot config)
        {
            if (validatorBuilder is null)
            {
                throw new ArgumentNullException(nameof(validatorBuilder), $"{nameof(validatorBuilder)} is null");
            }

            if (config is null)
            {
                throw new ArgumentNullException(nameof(config), $"{nameof(config)} is null");
            }

            var validatorParameters = ReadParamsFromJson(config.GetSection("custom"));
            return (CreateValidator(validatorParameters, validatorBuilder), validatorParameters);
        }

        /// <summary>
        /// Creates the default.
        /// </summary>
        /// <param name="validatorBuilder">The validator builder.</param>
        /// <param name="config">The configuration.</param>
        /// <returns>(IRecordValidator, ValidatorParameters).</returns>
        public static (IRecordValidator, ValidatorParameters) CreateDefault(this ValidatorBuilder validatorBuilder, IConfigurationRoot config)
        {
            if (validatorBuilder is null)
            {
                throw new ArgumentNullException(nameof(validatorBuilder), $"{nameof(validatorBuilder)} is null");
            }

            if (config is null)
            {
                throw new ArgumentNullException(nameof(config), $"{nameof(config)} is null");
            }

            var validatorParameters = ReadParamsFromJson(config.GetSection("default"));
            return (CreateValidator(validatorParameters, validatorBuilder), validatorParameters);
        }

        private static IRecordValidator CreateValidator(ValidatorParameters validatorParameters, ValidatorBuilder validatorBuilder)
        {
            IRecordValidator recordValidator = validatorBuilder
                .ValidateFirstName(validatorParameters.FirstNameCriterions.Min, validatorParameters.FirstNameCriterions.Max)
                .ValidateLastName(validatorParameters.LastNameCriterions.Min, validatorParameters.LastNameCriterions.Max)
                .ValidateGender()
                .ValidateDateOfBirth(validatorParameters.DateOfBirthCriterions.From, validatorParameters.DateOfBirthCriterions.To)
                .ValidateCreditSum(validatorParameters.CreditSumCriterions.Min, validatorParameters.CreditSumCriterions.Max)
                .ValidateDuration(validatorParameters.DurationCriterions.From, validatorParameters.DurationCriterions.To)
                .Create();
            return recordValidator;
        }

        private static ValidatorParameters ReadParamsFromJson(IConfigurationSection config)
        {
            var firstNameCriterion = new FirstNameCriterions();
            var lastNameCriterion = new LastNameCriterions();
            var dateOfBirthCriterion = new DateOfBirthCriterions();
            var durationCriterion = new DurationCriterions();
            var creditSumCriterion = new CreditSumCriterions();

            config.GetSection("firstName").Bind(firstNameCriterion);
            config.GetSection("lastName").Bind(lastNameCriterion);
            config.GetSection("dateOfBirth").Bind(dateOfBirthCriterion);
            config.GetSection("creditSum").Bind(creditSumCriterion);
            config.GetSection("duration").Bind(durationCriterion);

            return new ValidatorParameters(firstNameCriterion, lastNameCriterion, dateOfBirthCriterion, creditSumCriterion, durationCriterion);
        }
    }
}