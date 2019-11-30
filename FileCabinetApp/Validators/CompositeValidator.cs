using System.Collections.Generic;
using System.Linq;

using FileCabinetApp.Records;

namespace FileCabinetApp.Validators
{
    /// <summary>
    ///     CompositeValidator.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Validators.IRecordValidator" />
    public class CompositeValidator : IRecordValidator
    {
        private readonly List<IRecordValidator> validators;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CompositeValidator" /> class.
        /// </summary>
        /// <param name="recordValidators">The record validators.</param>
        public CompositeValidator(IEnumerable<IRecordValidator> recordValidators)
        {
            this.validators = recordValidators.ToList();
        }

        /// <summary>
        ///     Validates the parameters.
        /// </summary>
        /// <param name="record">The record.</param>
        public void ValidateParameters(FileCabinetRecord record)
        {
            foreach (var validator in this.validators)
            {
                validator.ValidateParameters(record);
            }
        }
    }
}