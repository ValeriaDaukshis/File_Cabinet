using System.Collections.Generic;

namespace FileCabinetApp.CommandHandlers.Printer
{
    public interface IRecordPrinter
    {
        void Print(IEnumerable<FileCabinetRecord> records);
    }
}