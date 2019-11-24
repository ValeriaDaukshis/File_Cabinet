using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FileCabinetApp.Converters;
using FileCabinetApp.FileReaders;
using FileCabinetApp.FIleWriters;
using FileCabinetApp.Records;
using FileCabinetApp.Validators;

namespace FileCabinetApp.Service
{
    /// <summary>
    /// FileCabinetServiceSnapshot.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private readonly FileCabinetRecord[] records;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="records">The records.</param>
        public FileCabinetServiceSnapshot(FileCabinetRecord[] records)
        {
            this.records = records;
        }

        /// <summary>
        /// Gets the read records.
        /// </summary>
        /// <value>
        /// The read records.
        /// </value>
        public IList<FileCabinetRecord> ReadRecords { get; private set; }

        /// <summary>
        /// Makes the snapshot.
        /// </summary>
        /// <returns>snapshot.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.records);
        }

        /// <summary>
        /// Saves to CSV.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void SaveToCsv(StreamWriter writer)
        {
            FileCabinetRecordCsvWriter writeToFile = new FileCabinetRecordCsvWriter(writer);
            Type t = typeof(FileCabinetRecord);
            PropertyInfo[] info = t.GetProperties();
            writeToFile.WriteFields(info);
            foreach (var record in this.records)
            {
                writeToFile.Write(record);
            }
        }

        /// <summary>
        /// Saves to XML.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void SaveToXml(StreamWriter writer)
        {
            FileCabinetRecordXmlWriter writeToFile = new FileCabinetRecordXmlWriter(writer);
            writeToFile.WriteHeader();
            foreach (var record in this.records)
            {
                writeToFile.Write(record);
            }

            writeToFile.WriteFooter();
        }

        /// <summary>
        /// Loads from CSV.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="validator">The validator.</param>
        /// <param name="converter">The converter.</param>
        /// <param name="consoleWriter">console writer.</param>
        public void LoadFromCsv(StreamReader reader, IRecordValidator validator, Converter converter, ConsoleWriters consoleWriter)
        {
            FileCabinetRecordCsvReader csvReader = new FileCabinetRecordCsvReader(reader, validator, converter, consoleWriter);
            this.ReadRecords = csvReader.ReadAll();
        }

        /// <summary>
        /// Loads from XML.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="validator">The validator.</param>
        /// <param name="consoleWriter">console writer.</param>
        public void LoadFromXml(StreamReader reader, IRecordValidator validator, ConsoleWriters consoleWriter)
        {
            FileCabinetRecordXmlReader xmlReader = new FileCabinetRecordXmlReader(reader, validator, consoleWriter);
            this.ReadRecords = xmlReader.ReadAll();
        }
    }
}