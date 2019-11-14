using System;
using System.Collections.Generic;
using FileCabinetApp.Records;

namespace FileCabinetApp.CommandHandlers.Printer
{
    /// <summary>
    /// DefaultRecordPrinter.
    /// </summary>
    /// <seealso cref="FileCabinetApp.CommandHandlers.Printer.IRecordPrinter" />
    public class DefaultRecordPrinter : IRecordPrinter
    {
        /// <summary>
        /// Prints the specified record.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns>true if records was printed.</returns>
        public bool Print(IEnumerable<FileCabinetRecord> record)
        {
            bool isPrinted = false;
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), $"{nameof(record)} is null");
            }

            foreach (var rec in record)
            {
                Console.WriteLine($"Id: {rec.Id}");
                Console.WriteLine($"\tFirst name: {rec.FirstName}");
                Console.WriteLine($"\tLast name: {rec.LastName}");
                Console.WriteLine($"\tGender: {rec.Gender}");
                Console.WriteLine($"\tDate of birth: {rec.DateOfBirth:yyyy-MMMM-dd}");
                Console.WriteLine($"\tCredit sum: {rec.CreditSum}");
                Console.WriteLine($"\tCredit duration: {rec.Duration}");
                isPrinted = true;
            }

            return isPrinted;
        }
    }
}