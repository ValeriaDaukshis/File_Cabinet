using System;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// DefaultValidator.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Validators.IRecordValidator" />
    public class DefaultValidator : IRecordValidator
    {
        /// <summary>
        /// Validates the first name.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        public void ValidateFirstName(string firstName)
        {
            if (string.IsNullOrEmpty(firstName))
            {
                throw new ArgumentNullException($"{nameof(firstName)}: First name is null");
            }

            if (firstName.Length < 2 || firstName.Length > 60)
            {
                throw new ArgumentException($"{nameof(firstName)}: First name length is upper than 60 or under than 2 symbols");
            }
        }

        /// <summary>
        /// Validates the last name.
        /// </summary>
        /// <param name="lastName">The last name.</param>
        public void ValidateLastName(string lastName)
        {
            if (string.IsNullOrEmpty(lastName))
            {
                throw new ArgumentNullException($"{nameof(lastName)}: First name is null");
            }

            if (lastName.Length < 2 || lastName.Length > 60)
            {
                throw new ArgumentException($"{nameof(lastName)}: First name length is upper than 60 or under than 2 symbols");
            }
        }

        /// <summary>
        /// Validates the gender.
        /// </summary>
        /// <param name="gender">The gender.</param>
        public void ValidateGender(char gender)
        {
            if (gender != 'M' && gender != 'F')
            {
                throw new ArgumentException($"{nameof(gender)}: Indefinite gender");
            }
        }

        /// <summary>
        /// Validates the date of birth.
        /// </summary>
        /// <param name="dateOfBirth">The date of birth.</param>
        public void ValidateDateOfBirth(DateTime dateOfBirth)
        {
            if (dateOfBirth > DateTime.Now)
            {
                throw new ArgumentException($"{nameof(dateOfBirth)}: Date of birth is upper than today's date");
            }

            if (dateOfBirth < new DateTime(1950, 01, 01))
            {
                throw new ArgumentException($"{nameof(dateOfBirth)}: Date of birth is under than 01-Jan-1950");
            }
        }

        /// <summary>
        /// Validates the duration.
        /// </summary>
        /// <param name="duration">The duration.</param>
        public void ValidateDuration(short duration)
        {
            if (duration > 120 || duration < 6)
            {
                throw new ArgumentException($"{nameof(duration)}: Credit duration is upper than 120 or under than 6 weeks");
            }
        }

        /// <summary>
        /// Validates the credit sum.
        /// </summary>
        /// <param name="creditSum">The credit sum.</param>
        public void ValidateCreditSum(decimal creditSum)
        {
            if (creditSum > 5000 || creditSum < 10)
            {
                throw new ArgumentException($"{nameof(creditSum)}: Credit sum is upper than 5000 or under than 10 BYN");
            }
        }
    }
}