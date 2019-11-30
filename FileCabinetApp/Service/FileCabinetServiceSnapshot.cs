using System.Collections.Generic;
using System.IO;

using FileCabinetApp.Converters;
using FileCabinetApp.FileReaders;
using FileCabinetApp.FIleWriters;
using FileCabinetApp.Records;
using FileCabinetApp.Validators;

namespace FileCabinetApp.Service
{
    /// <summary>
    ///     FileCabinetServiceSnapshot.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private readonly FileCabinetRecord[] records;

        /// <summary>
        ///     Initializes a new instance of the <see cref="FileCabinetServiceSnapshot" /> class.
        /// </summary>
        /// <param name="records">The records.</param>
        public FileCabinetServiceSnapshot(FileCabinetRecord[] records)
        {
            this.records = records;
        }

        /// <summary>
        ///     Gets the read records.
        /// </summary>
        /// <value>
        ///     The read records.
        /// </value>
        public IList<FileCabinetRecord> ReadRecords { get; private set; }

        /// <summary>
        ///     Loads from CSV.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="validator">The validator.</param>
        /// <param name="converter">The converter.</param>
        /// <param name="modelWriter">console writer.</param>
        public void LoadFromCsv(StreamReader reader, IRecordValidator validator, Converter converter, ModelWriters modelWriter)
        {
            var csvReader = new FileCabinetRecordCsvReader(reader, validator, converter, modelWriter);
            this.ReadRecords = csvReader.ReadAll();
        }

        /// <summary>
        ///     Loads from XML.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="validator">The validator.</param>
        /// <param name="modelWriter">console writer.</param>
        public void LoadFromXml(StreamReader reader, IRecordValidator validator, ModelWriters modelWriter)
        {
            var xmlReader = new FileCabinetRecordXmlReader(reader, validator, modelWriter);
            this.ReadRecords = xmlReader.ReadAll();
        }

        /// <summary>
        ///     Makes the snapshot.
        /// </summary>
        /// <returns>snapshot.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.records);
        }

        /// <summary>
        ///     Saves to CSV.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void SaveToCsv(StreamWriter writer)
        {
            var writeToFile = new FileCabinetRecordCsvWriter(writer);
            var t = typeof(FileCabinetRecord);
            var info = t.GetProperties();
            writeToFile.WriteFields(info);
            foreach (var record in this.records)
            {
                writeToFile.Write(record);
            }
        }

        /// <summary>
        ///     Saves to XML.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void SaveToXml(StreamWriter writer)
        {
            var writeToFile = new FileCabinetRecordXmlWriter(writer);
            writeToFile.WriteHeader();
            foreach (var record in this.records)
            {
                writeToFile.Write(record);
            }

            writeToFile.WriteFooter();
        }
    }
}