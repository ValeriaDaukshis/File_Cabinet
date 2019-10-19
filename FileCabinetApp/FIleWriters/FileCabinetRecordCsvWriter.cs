using System;
using System.IO;
using System.Reflection;

namespace FileCabinetApp.CSV
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
            this.writer.WriteLine($"{record.Id},{record.FirstName},{record.LastName},{record.Gender},{record.DateOfBirth:yyyy-MMMM-dd},{record.CreditSum},{record.Duration}");
        }
    }
}