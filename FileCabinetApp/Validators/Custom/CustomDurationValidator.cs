using System;

namespace FileCabinetApp.Validators.Custom
{
    public class CustomDurationValidator : IRecordValidator
    {
        public void ValidateParameters(FileCabinetRecord record)
        {
            short duration = record.Duration;
            if (duration > 500 || duration < 12)
            {
                throw new ArgumentException(nameof(duration), $"{nameof(duration)}: Credit duration is upper than 120 or under than 6 weeks");
            }
        }
    }
}