using System;

namespace FileCabinetApp.Validators
{
    public class DefaultDurationValidator : IRecordValidator
    {
        public void ValidateParameters(FileCabinetRecord record)
        {
            short duration = record.Duration;
            if (duration > 120 || duration < 6)
            {
                throw new ArgumentException(nameof(duration), $"{nameof(duration)}: Credit duration is upper than 120 or under than 6 weeks");
            }
        }
    }
}