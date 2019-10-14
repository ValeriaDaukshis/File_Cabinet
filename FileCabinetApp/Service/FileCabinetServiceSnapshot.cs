using System;
using System.IO;
using System.Reflection;
using FileCabinetApp.CSV;

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
    }
}