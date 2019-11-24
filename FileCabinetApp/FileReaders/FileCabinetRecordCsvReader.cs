using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using FileCabinetApp.Converters;
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
        private readonly Converter converter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvReader"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="validator">The validator.</param>
        /// <param name="converter">The converter.</param>
        public FileCabinetRecordCsvReader(StreamReader reader, IRecordValidator validator, Converter converter)
        {
            this.reader = reader;
            this.validator = validator;
            this.converter = converter;
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

        private static T ConvertValue<T>(Func<string, Tuple<bool, string, T>> converter, string input, int id)
        {
            T value;

            var conversionResult = converter(input);

            if (!conversionResult.Item1)
            {
                throw new ArgumentException($"Id #{id}: conversation failed ({conversionResult.Item2})");
            }

            value = conversionResult.Item3;
            return value;
        }

        private void ReaderValidator(string[] row, out int id, out string firstName, out string lastName, out char gender, out DateTime dateOfBirth, out decimal credit, out short duration)
        {
            id = ConvertValue<int>(this.converter.IntConverter, row[0], 0);
            firstName = ConvertValue<string>(this.converter.StringConverter, row[1], id);
            lastName = ConvertValue<string>(this.converter.StringConverter, row[2], id);
            gender = ConvertValue<char>(this.converter.CharConverter, row[3], id);
            dateOfBirth = ConvertValue<DateTime>(this.converter.DateTimeConverter, row[4], id);
            credit = ConvertValue<decimal>(this.converter.DecimalConverter, row[5], id);
            duration = ConvertValue<short>(this.converter.ShortConverter, row[6], id);

            this.validator.ValidateParameters(new FileCabinetRecord(id, firstName, lastName, gender, dateOfBirth, credit, duration));
        }
    }
}