using System;
using System.IO;

using Microsoft.Extensions.Configuration;

namespace FileCabinetApp.Validators
{
    /// <summary>
    ///     ValidatorsExtensions.
    /// </summary>
    public static class ValidatorsExtensions
    {
        /// <summary>
        ///     Creates the custom.
        /// </summary>
        /// <param name="validatorBuilder">The validator builder.</param>
        /// <param name="validationRulesFile">The validation rules file.</param>
        /// <returns>IRecordValidator.</returns>
        public static (IRecordValidator, ValidatorParameters) CreateCustom(this ValidatorBuilder validatorBuilder, string validationRulesFile)
        {
            if (string.IsNullOrEmpty(validationRulesFile))
            {
                throw new ArgumentNullException(nameof(validationRulesFile), $"{nameof(validationRulesFile)} is null");
            }

            return CreateValidator(validatorBuilder, "custom", validationRulesFile);
        }

        /// <summary>
        ///     Creates the default.
        /// </summary>
        /// <param name="validatorBuilder">The validator builder.</param>
        /// <param name="validationRulesFile">The validation rules file.</param>
        /// <returns>IRecordValidator.</returns>
        public static (IRecordValidator, ValidatorParameters) CreateDefault(this ValidatorBuilder validatorBuilder, string validationRulesFile)
        {
            if (string.IsNullOrEmpty(validationRulesFile))
            {
                throw new ArgumentNullException(nameof(validationRulesFile), $"{nameof(validationRulesFile)} is null");
            }

            return CreateValidator(validatorBuilder, "default", validationRulesFile);
        }

        private static (IRecordValidator, ValidatorParameters) CreateValidator(ValidatorBuilder validatorBuilder, string name, string validationRulesFile)
        {
            if (validatorBuilder is null)
            {
                throw new ArgumentNullException(nameof(validatorBuilder), $"{nameof(validatorBuilder)} is null");
            }

            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile(validationRulesFile);
            var config = builder.Build();

            var validationRules = config.GetSection(name).Get<ValidatorParameters>();

            var recordValidator = validatorBuilder.ValidateFirstName(validationRules.FirstName.Min, validationRules.FirstName.Max)
                                                  .ValidateLastName(validationRules.LastName.Min, validationRules.LastName.Max)
                                                  .ValidateGender()
                                                  .ValidateDateOfBirth(validationRules.DateOfBirth.From, validationRules.DateOfBirth.To)
                                                  .ValidateCreditSum(validationRules.CreditSum.Min, validationRules.CreditSum.Max)
                                                  .ValidateDuration(validationRules.Duration.From, validationRules.Duration.To)
                                                  .Create();
            return (recordValidator, validationRules);
        }
    }
}