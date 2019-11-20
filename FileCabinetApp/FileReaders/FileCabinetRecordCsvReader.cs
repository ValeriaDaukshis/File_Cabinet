using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using FileCabinetApp.Records;
using FileCabinetApp.Validators;

namespace FileCabinetApp.FileReaders
{
    /// <summary>
    /// FileCabinetRecordCsvReader.
    /// </summary>
    public class FileCabinetRecordCsvReader
    {
        private readonly StreamReader reader;
        private readonly IRecordValidator validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvReader"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="validator">The validator.</param>
        public FileCabinetRecordCsvReader(StreamReader reader, IRecordValidator validator)
        {
            this.reader = reader;
            this.validator = validator;
        }

        /// <summary>
        /// Reads all.
        /// </summary>
        /// <returns>list of records.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            IList<FileCabinetRecord> records = new List<FileCabinetRecord>();
            string line;
            while ((line = this.reader.ReadLine()) != null)
            {
                string[] row = line.Split(',');
                if (row[0] == "id")
                {
                    continue;
                }

                try
                {
                    this.ReaderValidator(row, out int id, out string firstName, out string lastName, out char gender, out DateTime dateOfBirth, out decimal credit, out short duration);
                    records.Add(new FileCabinetRecord(id, firstName, lastName, gender, dateOfBirth, credit, duration));
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return records;
        }

        private void ReaderValidator(string[] row, out int id, out string firstName, out string lastName, out char gender, out DateTime dateOfBirth, out decimal credit, out short duration)
        {
            firstName = string.Empty;
            lastName = string.Empty;
            if (!int.TryParse(row[0], out id))
            {
                throw new ArgumentException($" {row[0]} Id has not digit format");
            }

            if (string.IsNullOrEmpty(row[1]))
            {
                throw new ArgumentException($"{id} firstName is null or empty");
            }

            firstName = row[1];

            if (string.IsNullOrEmpty(row[2]))
            {
                throw new ArgumentException($"{id} lastName is null or empty");
            }

            lastName = row[2];

            if (!char.TryParse(row[3], out gender))
            {
                throw new ArgumentException($"{id} gender is not char");
            }

            if (!DateTime.TryParse(row[4], CultureInfo.InvariantCulture, DateTimeStyles.None, out dateOfBirth))
            {
                throw new ArgumentException($"{id} Incorrect DateTime value");
            }

            if (!decimal.TryParse(row[5], out credit))
            {
                throw new ArgumentException($"{id} creditSum has no decimal format");
            }

            if (!short.TryParse(row[6], out duration))
            {
                throw new ArgumentException($"{id} duration has not short format");
            }

            this.validator.ValidateParameters(new FileCabinetRecord(id, firstName, lastName, gender, dateOfBirth, credit, duration));
        }
    }
}