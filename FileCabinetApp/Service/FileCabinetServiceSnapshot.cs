using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FileCabinetApp.FileReaders;
using FileCabinetApp.FIleWriters;
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

        public IList<FileCabinetRecord> ReadRecords { get; set; }

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
        public void LoadFromCsv(StreamReader reader, IRecordValidator validator)
        {
            FileCabinetRecordCsvReader csvReader = new FileCabinetRecordCsvReader(reader, validator);
            this.ReadRecords = csvReader.ReadAll();
        }
    }
}