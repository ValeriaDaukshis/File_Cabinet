using System;
using FileCabinetApp.CommandHandlers;

namespace FileCabinetApp.Validators
{
    public static class ValidatorsExtensions
    {
        public static (IRecordValidator, ValidatorParameters) CreateCustom(this ValidatorBuilder validatorBuilder)
        {
            var validatorParameters = new ValidatorParameters(2, 60, new DateTime(1930, 1, 1), DateTime.Now, 100, 5000, 6, 120);
            return (CreateValidator(validatorParameters, validatorBuilder), validatorParameters);
        }

        public static (IRecordValidator, ValidatorParameters) CreateDefault(this ValidatorBuilder validatorBuilder)
        {
            var validatorParameters = new ValidatorParameters(4, 60, new DateTime(1950, 1, 1), DateTime.Now, 100, 10_000, 6, 500);
            return (CreateValidator(validatorParameters, validatorBuilder), validatorParameters);
        }

        private static IRecordValidator CreateValidator(ValidatorParameters validatorParameters, ValidatorBuilder validatorBuilder)
        {
            IRecordValidator recordValidator =validatorBuilder
                .ValidateFirstName(validatorParameters.minLength, validatorParameters.maxLength)
                .ValidateLastName(validatorParameters.minLength, validatorParameters.maxLength)
                .ValidateGender()
                .ValidateDateOfBirth(validatorParameters.minDateOfBirth, validatorParameters.maxDateOfBirth)
                .ValidateCreditSum(validatorParameters.minCreditSum, validatorParameters.maxCreditSum)
                .ValidateDuration(validatorParameters.minPeriod, validatorParameters.maxPeriod)
                .Create();
            return recordValidator;
        }
    }
}