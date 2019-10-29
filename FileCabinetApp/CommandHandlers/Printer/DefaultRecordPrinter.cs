using System;
using System.Collections.Generic;

namespace FileCabinetApp.CommandHandlers.Printer
{
    public class DefaultRecordPrinter : IRecordPrinter
    {
        public void Print(IEnumerable<FileCabinetRecord> record)
        {
            foreach (var rec in record)
            {
                Console.WriteLine($"Id: {rec.Id}");
                Console.WriteLine($"\tFirst name: {rec.FirstName}");
                Console.WriteLine($"\tLast name: {rec.LastName}");
                Console.WriteLine($"\tGender: {rec.Gender}");
                Console.WriteLine($"\tDate of birth: {rec.DateOfBirth:yyyy-MMMM-dd}");
                Console.WriteLine($"\tCredit sum: {rec.CreditSum}");
                Console.WriteLine($"\tCredit duration: {rec.Duration}");
            }
        }
    }
}