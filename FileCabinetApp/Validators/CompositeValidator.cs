using System.Collections.Generic;
using System.Linq;

namespace FileCabinetApp.Validators
{
    public class CompositeValidator : IRecordValidator
    {
        private List<IRecordValidator> validators;

        public CompositeValidator(IEnumerable<IRecordValidator> recordValidators)
        {
            this.validators = recordValidators.ToList();
        }

        public void ValidateParameters(FileCabinetRecord record)
        {
            foreach (var validator in this.validators)
            {
                validator.ValidateParameters(record);
            }
        }
    }
}