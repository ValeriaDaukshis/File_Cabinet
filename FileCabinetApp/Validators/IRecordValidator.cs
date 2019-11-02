using System;
using FileCabinetApp.Records;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// IRecordValidator.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// Validates the parameters.
        /// </summary>
        /// <param name="record">The record.</param>
        void ValidateParameters(FileCabinetRecord record);
    }
}