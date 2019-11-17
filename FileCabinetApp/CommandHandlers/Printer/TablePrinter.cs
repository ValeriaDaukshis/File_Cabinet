using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using FileCabinetApp.Records;

namespace FileCabinetApp.CommandHandlers.Printer
{
    /// <summary>
    /// TablePrinter.
    /// </summary>
    public class TablePrinter : ITablePrinter
    {
        private static string angle = "+";
        private static string wall = "|";
        private static string border = "-";

        /// <summary>
        /// Prints the specified records.
        /// </summary>
        /// <param name="record">The records.</param>
        /// <param name="fields">The fields.</param>
        public void Print(IEnumerable<FileCabinetRecord> record, string[] fields)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), $"{nameof(record)} is null");
            }

            if (fields is null)
            {
                throw new ArgumentNullException(nameof(fields), $"{nameof(fields)} is null");
            }

            if (!record.Any())
            {
                Console.WriteLine("No records found");
                return;
            }

            string horizontalBorder = CreateHorizontalBorder(record, fields);
            Console.WriteLine(horizontalBorder);
            Console.WriteLine(CreateHeader(fields, horizontalBorder));
            Console.WriteLine(horizontalBorder);

            foreach (var rec in record)
            {
                Console.WriteLine(PrintRecord(rec, fields));
            }

            Console.WriteLine(horizontalBorder);
        }

        private static string PrintRecord(FileCabinetRecord record, string[] fields)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(wall);
            for (int i = 0; i < fields.Length; i++)
            {
                var value = record.GetType().GetProperty(fields[i])?.GetValue(record, null);
                if (value is null)
                {
                    continue;
                }

                builder.Append(" ");
                if (value is string || value is char)
                {
                    builder.Append(PrintStringValues(fields[i], value.ToString()));
                }
                else if (value is DateTime)
                {
                    builder.Append(PrintDateTimeValue(fields[i], (DateTime)value));
                }
                else
                {
                    builder.Append(PrintDigitValues(fields[i], value.ToString()));
                }

                builder.Append(" ");
                builder.Append(wall);
            }

            return builder.ToString();
        }

        private static string CreateHorizontalBorder(IEnumerable<FileCabinetRecord> record, string[] fields)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(angle);
            for (int i = 0; i < fields.Length; i++)
            {
                builder.Append(border);
                for (int j = 0; j < CheckRecordLength(record, fields[i]); j++)
                {
                    builder.Append(border);
                }

                builder.Append(border);
                builder.Append(angle);
            }

            return builder.ToString();
        }

        private static int CheckRecordLength(IEnumerable<FileCabinetRecord> records, string field)
        {
            int maxLength = field.Length;
            foreach (var record in records)
            {
                var value = record.GetType().GetProperty(field)?.GetValue(record, null);
                int len;
                if (value is null)
                {
                    continue;
                }

                if (value is DateTime)
                {
                    len = ((DateTime)value).ToString("yyyy-MMMM-dd", CultureInfo.InvariantCulture).Length;
                }
                else
                {
                    len = value.ToString().Length;
                }

                maxLength = len > maxLength ? len : maxLength;
            }

            return maxLength;
        }

        private static string CreateHeader(string[] fields, string horizontalBorder)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(wall);
            string[] a = horizontalBorder.Split("+", StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < fields.Length; i++)
            {
                builder.Append(" ");
                builder.Append(fields[i]);
                builder.Append(" ");
                for (int j = 0; j < a[i].Length - fields[i].Length - 2; j++)
                {
                    builder.Append(" ");
                }

                builder.Append(wall);
            }

            return builder.ToString();
        }

        private static string PrintStringValues(string fields, string values)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(values);
            for (int j = 0; j < fields.Length - values.Length; j++)
            {
                builder.Append(" ");
            }

            return builder.ToString();
        }

        private static string PrintDigitValues(string fields, string values)
        {
            StringBuilder builder = new StringBuilder();
            for (int j = 0; j < fields.Length - values.Length; j++)
            {
                builder.Append(" ");
            }

            builder.Append(values);

            return builder.ToString();
        }

        private static string PrintDateTimeValue(string fields, DateTime values)
        {
            StringBuilder builder = new StringBuilder();
            var dateOfBirth = values.ToString("yyyy-MMMM-dd", CultureInfo.InvariantCulture);
            for (int j = 0; j < fields.Length - dateOfBirth.Length; j++)
            {
                builder.Append(" ");
            }

            builder.Append($"{dateOfBirth}");

            return builder.ToString();
        }
    }
}