using System;
using FileCabinetApp.Validators.ValidationParameters;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// ValidatorParameters.
    /// </summary>
    public class ValidatorParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatorParameters"/> class.
        /// </summary>
        /// <param name="firstNameCriterions">The first name criterions.</param>
        /// <param name="lastNameCriterions">The last name criterions.</param>
        /// <param name="dateOfBirthCriterions">The date of birth criterions.</param>
        /// <param name="creditSumCriterions">The credit sum criterions.</param>
        /// <param name="durationCriterions">The duration criterions.</param>
        public ValidatorParameters(FirstNameCriterions firstNameCriterions, LastNameCriterions lastNameCriterions, DateOfBirthCriterions dateOfBirthCriterions, CreditSumCriterions creditSumCriterions, DurationCriterions durationCriterions)
        {
            this.DurationCriterions = durationCriterions;
            this.FirstNameCriterions = firstNameCriterions;
            this.LastNameCriterions = lastNameCriterions;
            this.DateOfBirthCriterions = dateOfBirthCriterions;
            this.CreditSumCriterions = creditSumCriterions;
        }

        /// <summary>
        /// Gets the last name criterions.
        /// </summary>
        /// <value>
        /// The last name criterions.
        /// </value>
        public LastNameCriterions LastNameCriterions { get; private set; }

        /// <summary>
        /// Gets the first name criterions.
        /// </summary>
        /// <value>
        /// The first name criterions.
        /// </value>
        public FirstNameCriterions FirstNameCriterions { get; private set; }

        /// <summary>
        /// Gets the duration criterions.
        /// </summary>
        /// <value>
        /// The duration criterions.
        /// </value>
        public DurationCriterions DurationCriterions { get; private set; }

        /// <summary>
        /// Gets the credit sum criterions.
        /// </summary>
        /// <value>
        /// The credit sum criterions.
        /// </value>
        public CreditSumCriterions CreditSumCriterions { get; private set; }

        /// <summary>
        /// Gets the date of birth criterions.
        /// </summary>
        /// <value>
        /// The date of birth criterions.
        /// </value>
        public DateOfBirthCriterions DateOfBirthCriterions { get; private set; }
    }
}