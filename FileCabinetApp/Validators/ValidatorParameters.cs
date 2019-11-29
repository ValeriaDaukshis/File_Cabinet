using System;
using FileCabinetApp.Validators.ValidationModel;
using FileCabinetApp.Validators.ValidationParameters;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// ValidatorParameters.
    /// </summary>
    public class ValidatorParameters
    {
        /// <summary>
        /// Gets or sets the last name criterions.
        /// </summary>
        /// <value>
        /// The last name criterions.
        /// </value>
        public LastName LastName { get;  set; }

        /// <summary>
        /// Gets or sets the first name criterions.
        /// </summary>
        /// <value>
        /// The first name criterions.
        /// </value>
        public FirstName FirstName { get;  set; }

        /// <summary>
        /// Gets or sets the duration criterions.
        /// </summary>
        /// <value>
        /// The duration criterions.
        /// </value>
        public Duration Duration { get;  set; }

        /// <summary>
        /// Gets or sets the credit sum criterions.
        /// </summary>
        /// <value>
        /// The credit sum criterions.
        /// </value>
        public CreditSum CreditSum { get;  set; }

        /// <summary>
        /// Gets or sets the date of birth criterions.
        /// </summary>
        /// <value>
        /// The date of birth criterions.
        /// </value>
        public DateOfBirth DateOfBirth { get;  set; }
    }
}