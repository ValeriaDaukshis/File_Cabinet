using System;

using FileCabinetApp.Records;

namespace FileCabinetApp.Validators.CommonValidators
{
    /// <summary>
    ///     CreditSumValidator.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Validators.IRecordValidator" />
    public class CreditSumValidator : IRecordValidator
    {
        private readonly decimal maxCreditSum;

        private readonly decimal minCreditSum;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CreditSumValidator" /> class.
        /// </summary>
        /// <param name="minCreditSum">The minimum credit sum.</param>
        /// <param name="maxCreditSum">The maximum credit sum.</param>
        public CreditSumValidator(decimal minCreditSum, decimal maxCreditSum)
        {
            this.minCreditSum = minCreditSum;
            this.maxCreditSum = maxCreditSum;
        }

        /// <summary>
        ///     Validates the parameters.
        /// </summary>
        /// <param name="record">The record.</param>
        public void ValidateParameters(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), $"{nameof(record)} is null");
            }

            var creditSum = record.CreditSum;
            if (creditSum > this.maxCreditSum || creditSum < this.minCreditSum)
            {
                throw new ArgumentException($"Id #{record.Id}: Credit sum is upper than {this.maxCreditSum} or under than {this.minCreditSum} BYN ({nameof(creditSum)})");
            }
        }
    }
}