﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using FileCabinetApp.Records;

namespace FileCabinetApp.Printer
{
    /// <summary>
    ///     TablePrinter.
    /// </summary>
    public class DefaultTablePrinter : ITablePrinter
    {
        private const string Angle = "+";

        private const string Border = "-";

        private const string Wall = "|";

        private readonly Action<string> consoleWriter;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultTablePrinter" /> class.
        /// </summary>
        /// <param name="consoleWriter">The console writer.</param>
        public DefaultTablePrinter(Action<string> consoleWriter)
        {
            this.consoleWriter = consoleWriter;
        }

        /// <summary>
        ///     Prints the specified records.
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
                this.consoleWriter.Invoke("No records found");
                return;
            }

            var horizontalBorder = CreateHorizontalBorder(record, fields);
            this.consoleWriter.Invoke(horizontalBorder);
            this.consoleWriter.Invoke(CreateHeader(fields, horizontalBorder));
            this.consoleWriter.Invoke(horizontalBorder);

            foreach (var rec in record)
            {
                this.consoleWriter.Invoke(PrintRecord(rec, fields, horizontalBorder));
            }

            this.consoleWriter.Invoke(horizontalBorder);
        }

        private static int CheckRecordLength(IEnumerable<FileCabinetRecord> records, string field)
        {
            var maxLength = field.Length;
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
            var builder = new StringBuilder();
            builder.Append(Wall);
            var recordsLength = horizontalBorder.Split("+", StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < fields.Length; i++)
            {
                builder.Append(" ");
                builder.Append(fields[i]);
                builder.Append(" ");
                for (var j = 0; j < recordsLength[i].Length - fields[i].Length - 2; j++)
                {
                    builder.Append(" ");
                }

                builder.Append(Wall);
            }

            return builder.ToString();
        }

        private static string CreateHorizontalBorder(IEnumerable<FileCabinetRecord> record, string[] fields)
        {
            var builder = new StringBuilder();
            builder.Append(Angle);
            for (var i = 0; i < fields.Length; i++)
            {
                builder.Append(Border);
                for (var j = 0; j < CheckRecordLength(record, fields[i]); j++)
                {
                    builder.Append(Border);
                }

                builder.Append(Border);
                builder.Append(Angle);
            }

            return builder.ToString();
        }

        private static string PrintDateTimeValue(string values, int horizontalBorderLength)
        {
            var builder = new StringBuilder();
            for (var j = 0; j < horizontalBorderLength; j++)
            {
                builder.Append(" ");
            }

            builder.Append($"{values}");

            return builder.ToString();
        }

        private static string PrintDigitValues(object values, int horizontalBorderLength)
        {
            var builder = new StringBuilder();
            for (var j = 0; j < horizontalBorderLength; j++)
            {
                builder.Append(" ");
            }

            var value = Convert.ToString(values, CultureInfo.InvariantCulture);
            builder.Append(value);

            return builder.ToString();
        }

        private static string PrintRecord(FileCabinetRecord record, string[] fields, string horizontalBorder)
        {
            var builder = new StringBuilder();
            builder.Append(Wall);
            var recordsLength = horizontalBorder.Split("+", StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < fields.Length; i++)
            {
                var value = record.GetType().GetProperty(fields[i])?.GetValue(record, null);
                if (value is null)
                {
                    continue;
                }

                builder.Append(" ");
                var stringValue = value.ToString();
                if (value is DateTime time)
                {
                    stringValue = time.ToString("yyyy-MMMM-dd", CultureInfo.InvariantCulture);
                }

                var horizontalBorderLength = recordsLength[i].Length - stringValue.Length - 2;
                if (value is string || value is char)
                {
                    builder.Append(PrintStringValues(stringValue, horizontalBorderLength));
                }
                else if (value is DateTime)
                {
                    builder.Append(PrintDateTimeValue(stringValue, horizontalBorderLength));
                }
                else
                {
                    builder.Append(PrintDigitValues(value, horizontalBorderLength));
                }

                builder.Append(" ");
                builder.Append(Wall);
            }

            return builder.ToString();
        }

        private static string PrintStringValues(string values, int horizontalBorderLength)
        {
            var builder = new StringBuilder();
            builder.Append(values);
            for (var j = 0; j < horizontalBorderLength; j++)
            {
                builder.Append(" ");
            }

            return builder.ToString();
        }
    }
}