using System;

namespace FileCabinetApp.Validators.Custom
{
    public class DurationValidator : IRecordValidator
    {
        private short minPeriod;
        private short maxPeriod;

        public DurationValidator(short minPeriod, short maxPeriod)
        {
            this.minPeriod = minPeriod;
            this.maxPeriod = maxPeriod;
        }

        public void ValidateParameters(FileCabinetRecord record)
        {
            short duration = record.Duration;
            if (duration > this.maxPeriod || duration < this.minPeriod)
            {
                throw new ArgumentException(nameof(duration), $"{nameof(duration)}: Credit duration is upper than {this.maxPeriod} or under than {this.minPeriod} weeks");
            }
        }
    }
}