using System;
using System.Collections.Generic;
using System.IO;

using FileCabinetApp.Converters;
using FileCabinetApp.Records;
using FileCabinetApp.Validators;

namespace FileCabinetApp.FileReaders
{
    /// <summary>
    ///     FileCabinetRecordCsvReader.
    /// </summary>
    public class FileCabinetRecordCsvReader
    {
        private readonly Converter converter;

        private readonly ModelWriters modelWriter;

        private readonly StreamReader reader;

        private readonly IRecordValidator validator;

        /// <summary>
        ///     Initializes a new instance of the <see cref="FileCabinetRecordCsvReader" /> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="validator">The validator.</param>
        /// <param name="converter">The converter.</param>
        /// <param name="modelWriter">console writer.</param>
        public FileCabinetRecordCsvReader(StreamReader reader, IRecordValidator validator, Converter converter, ModelWriters modelWriter)
        {
            this.reader = reader;
            this.validator = validator;
            this.converter = converter;
            this.modelWriter = modelWriter;
        }

        /// <summary>
        ///     Reads all.
        /// </summary>
        /// <returns>list of records.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            IList<FileCabinetRecord> records = new List<FileCabinetRecord>();
            string line;
            while ((line = this.reader.ReadLine()) != null)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    var row = line.Split(',');
                    if (row[0] == "id")
                    {
                        continue;
                    }

                    try
                    {
                        var record = this.ValidateParameters(row);
                        this.validator.ValidateParameters(record);
                        records.Add(record);
                    }
                    catch (ArgumentNullException ex)
                    {
                        this.modelWriter.LineWriter.Invoke(ex.Message);
                    }
                    catch (ArgumentException ex)
                    {
                        this.modelWriter.LineWriter.Invoke(ex.Message);
                    }
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

        private FileCabinetRecord ValidateParameters(string[] row)
        {
            var id = ConvertValue(this.converter.IntConverter, row[0], 0);
            var firstName = ConvertValue(this.converter.StringConverter, row[1], id);
            var lastName = ConvertValue(this.converter.StringConverter, row[2], id);
            var gender = ConvertValue(this.converter.CharConverter, row[3], id);
            var dateOfBirth = ConvertValue(this.converter.DateTimeConverter, row[4], id);
            var credit = ConvertValue(this.converter.DecimalConverter, row[5], id);
            var duration = ConvertValue(this.converter.ShortConverter, row[6], id);

            return new FileCabinetRecord(id, firstName, lastName, gender, dateOfBirth, credit, duration);
        }
    }
}