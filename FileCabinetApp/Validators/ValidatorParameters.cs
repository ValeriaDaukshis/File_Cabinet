using System;

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
        /// <param name="minLength">The minimum length.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <param name="minDateOfBirth">The minimum date of birth.</param>
        /// <param name="maxDateOfBirth">The maximum date of birth.</param>
        /// <param name="minCreditSum">The minimum credit sum.</param>
        /// <param name="maxCreditSum">The maximum credit sum.</param>
        /// <param name="minPeriod">The minimum period.</param>
        /// <param name="maxPeriod">The maximum period.</param>
        public ValidatorParameters(int minLength, int maxLength, DateTime minDateOfBirth, DateTime maxDateOfBirth, decimal minCreditSum, decimal maxCreditSum, short minPeriod, short maxPeriod)
        {
            this.MinLength = minLength;
            this.MaxLength = maxLength;
            this.MinDateOfBirth = minDateOfBirth;
            this.MaxDateOfBirth = maxDateOfBirth;
            this.MinCreditSum = minCreditSum;
            this.MaxCreditSum = maxCreditSum;
            this.MinPeriod = minPeriod;
            this.MaxPeriod = maxPeriod;
        }

        /// <summary>
        /// Gets the minimum credit sum.
        /// </summary>
        /// <value>
        /// The minimum credit sum.
        /// </value>
        public decimal MinCreditSum { get; private set; }

        /// <summary>
        /// Gets the maximum credit sum.
        /// </summary>
        /// <value>
        /// The maximum credit sum.
        /// </value>
        public decimal MaxCreditSum { get; private set; }

        /// <summary>
        /// Gets the minimum length.
        /// </summary>
        /// <value>
        /// The minimum length.
        /// </value>
        public int MinLength { get; private set; }

        /// <summary>
        /// Gets the maximum length.
        /// </summary>
        /// <value>
        /// The maximum length.
        /// </value>
        public int MaxLength { get; private set; }

        /// <summary>
        /// Gets the minimum period.
        /// </summary>
        /// <value>
        /// The minimum period.
        /// </value>
        public short MinPeriod { get; private set; }

        /// <summary>
        /// Gets the maximum period.
        /// </summary>
        /// <value>
        /// The maximum period.
        /// </value>
        public short MaxPeriod { get; private set; }

        /// <summary>
        /// Gets the minimum date of birth.
        /// </summary>
        /// <value>
        /// The minimum date of birth.
        /// </value>
        public DateTime MinDateOfBirth { get; private set; }

        /// <summary>
        /// Gets the maximum date of birth.
        /// </summary>
        /// <value>
        /// The maximum date of birth.
        /// </value>
        public DateTime MaxDateOfBirth { get; private set; }
    }
}