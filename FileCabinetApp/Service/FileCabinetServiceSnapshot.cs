﻿using System;
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
            return new FileCabinetServiceSnapshot(records);
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
            foreach (var record in records)
            {
                writeToFile.Write(record);
            }
        }
    }
}