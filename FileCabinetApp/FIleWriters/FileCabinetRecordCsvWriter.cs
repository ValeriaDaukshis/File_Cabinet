using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using FileCabinetApp.Records;

namespace FileCabinetApp.FIleWriters
{
    /// <summary>
    /// FileCabinetRecordCsvWriter.
    /// </summary>
    public class FileCabinetRecordCsvWriter
    {
        private readonly StreamWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvWriter"/> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public FileCabinetRecordCsvWriter(StreamWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Writes the fields.
        /// </summary>
        /// <param name="info">The information.</param>
        public void WriteFields(PropertyInfo[] info)
        {
            if (info is null)
            {
                throw new ArgumentNullException($"{nameof(info)} is null");
            }

            for (int i = 0; i < info.Length - 1; i++)
            {
                this.writer.Write($"{info[i].Name},");
            }

            this.writer.Write($"{info[info.Length - 1].Name}\n");
        }

        /// <summary>
        /// Writes the specified record.
        /// </summary>
        /// <param name="record">The record.</param>
        public void Write(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException($"{nameof(record)} is null");
            }

            var builder = new StringBuilder();
            builder.Append($"{record.Id},");
            builder.Append($"{record.FirstName},");
            builder.Append($"{record.LastName},");
            builder.Append($"{record.Gender},");
            builder.Append($"{record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)},");
            builder.Append($"{record.CreditSum.ToString(CultureInfo.InvariantCulture)},");
            builder.Append($"{record.Duration}");
            this.writer.WriteLine(builder.ToString());
        }
    }
}