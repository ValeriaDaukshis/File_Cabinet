using System;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// IRecordValidator.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// Validates the first name.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        void ValidateFirstName(string firstName);

        /// <summary>
        /// Validates the last name.
        /// </summary>
        /// <param name="lastName">The last name.</param>
        void ValidateLastName(string lastName);

        /// <summary>
        /// Validates the credit sum.
        /// </summary>
        /// <param name="creditSum">The credit sum.</param>
        void ValidateCreditSum(decimal creditSum);

        /// <summary>
        /// Validates the duration.
        /// </summary>
        /// <param name="duration">The duration.</param>
        void ValidateDuration(short duration);

        /// <summary>
        /// Validates the date of birth.
        /// </summary>
        /// <param name="dateOfBirth">The date of birth.</param>
        void ValidateDateOfBirth(DateTime dateOfBirth);

        /// <summary>
        /// Validates the gender.
        /// </summary>
        /// <param name="gender">The gender.</param>
        void ValidateGender(char gender);
    }
}