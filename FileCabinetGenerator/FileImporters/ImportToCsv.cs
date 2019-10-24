using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace FileCabinetGenerator.FileImporters
{
    /// <summary>
    /// ImportToCsv.
    /// </summary>
    public class ImportToCsv : IFileCabinetSerializer
    {
        private readonly StreamWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportToCsv"/> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public ImportToCsv(StreamWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Serializes the specified record.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Serialize(FileCabinetRecords record)
        {
            if (record is null)
            {
                throw new ArgumentNullException($"{nameof(record)} is null");
            }

            WriteHeader();

            for (int i = 0; i < record.Record.Count; i++)
            {
                var builder = new StringBuilder();
                builder.Append($"{record.Record[i].Id},");
                builder.Append($"{record.Record[i].Name.FirstName},");
                builder.Append($"{record.Record[i].Name.LastName},");
                builder.Append($"{record.Record[i].Gender},");
                builder.Append($"{record.Record[i].DateOfBirth.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)},");
                builder.Append($"{record.Record[i].CreditSum},");
                builder.Append($"{record.Record[i].Duration}");
                this.writer.WriteLine(builder.ToString());
            }
        }

        private void WriteHeader()
        {
            var names = new StringBuilder();
            names.Append("id,");
            names.Append("firstName,");
            names.Append("lastName,");
            names.Append("gender,");
            names.Append("dateOfBirth,");
            names.Append("creditSum,");
            names.Append("duration");
            this.writer.WriteLine(names.ToString());
        }
    }
}