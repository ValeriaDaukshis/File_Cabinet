using System.Collections.Generic;
using FileCabinetApp.Records;

namespace FileCabinetApp.CommandHandlers.Printer
{
    /// <summary>
    /// IRecordPrinter.
    /// </summary>
    public interface IRecordPrinter
    {
        /// <summary>
        /// Prints the specified records.
        /// </summary>
        /// <param name="records">The records.</param>
        void Print(IEnumerable<FileCabinetRecord> records);
    }
}