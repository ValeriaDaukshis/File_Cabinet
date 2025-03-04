﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FileCabinetApp.Records;

namespace FileCabinetApp.Service
{
    /// <summary>
    /// IFileCabinetService.
    /// </summary>
    public interface IFileCabinetService
    {
        /// <summary>
        /// Creates the record.
        /// </summary>
        /// <param name="record"> record.</param>
        /// <returns>Id of created record.</returns>
        int CreateRecord(FileCabinetRecord record);

        /// <summary>
        /// Edits the record.
        /// </summary>
        /// <param name="fileCabinetRecord">The file cabinet record.</param>
        void EditRecord(FileCabinetRecord fileCabinetRecord);

        /// <summary>
        /// Gets the stat.
        /// </summary>
        /// <returns>number of records.</returns>
        (int, int) GetStat();

        /// <summary>
        /// Gets the records.
        /// </summary>
        /// <returns>All records.</returns>
        ReadOnlyCollection<FileCabinetRecord> GetRecords();

        /// <summary>
        /// Makes the snapshot.
        /// </summary>
        /// <returns>FileCabinetServiceSnapshot.</returns>
        FileCabinetServiceSnapshot MakeSnapshot();

        /// <summary>
        /// Restores the specified snapshot.
        /// </summary>
        /// <param name="snapshot">The snapshot.</param>
        /// <returns>count of restored records.</returns>
        int Restore(FileCabinetServiceSnapshot snapshot);

        /// <summary>
        /// Removes the record.
        /// </summary>
        /// <param name="fileCabinetRecord">The file cabinet record.</param>
        /// <returns>record id.</returns>
        int RemoveRecord(FileCabinetRecord fileCabinetRecord);

        /// <summary>
        /// Purges the deleted records.
        /// </summary>
        /// <returns>count of deleted records and count of all records.</returns>
        (int, int) PurgeDeletedRecords();
    }
}