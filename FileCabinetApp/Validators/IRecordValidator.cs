using System;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// IRecordValidator.
    /// </summary>
    public interface IRecordValidator
    {
        void ValidateParameters(FileCabinetRecord record);
    }
}