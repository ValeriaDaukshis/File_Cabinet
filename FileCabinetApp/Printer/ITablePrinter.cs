using System.Collections.Generic;
using FileCabinetApp.Records;

namespace FileCabinetApp.CommandHandlers.Printer
{
    /// <summary>
    /// ITablePrinter.
    /// </summary>
    public interface ITablePrinter
    {
        /// <summary>
        /// Prints the specified records.
        /// </summary>
        /// <param name="record">The records.</param>
        /// <param name="fields">The fields.</param>
        void Print(IEnumerable<FileCabinetRecord> record, string[] fields);
    }
}