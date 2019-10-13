using System;
using System.Collections.ObjectModel;

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
        /// <param name="fileCabinetRecord">The file cabinet record.</param>
        /// <returns>Id of created record.</returns>
        int CreateRecord(FileCabinetRecord fileCabinetRecord);

        /// <summary>
        /// Edits the record.
        /// </summary>
        /// <param name="fileCabinetRecord">The file cabinet record.</param>
        void EditRecord(FileCabinetRecord fileCabinetRecord);

        /// <summary>
        /// Gets the stat.
        /// </summary>
        /// <returns>number of records.</returns>
        int GetStat();

        /// <summary>
        /// Gets the records.
        /// </summary>
        /// <returns>All records.</returns>
        ReadOnlyCollection<FileCabinetRecord> GetRecords();

        /// <summary>
        /// Finds the by date of birth.
        /// </summary>
        /// <param name="dateOfBirth">The date of birth.</param>
        /// <returns>Array of records.</returns>
        ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth);

        /// <summary>
        /// Finds the last name of the by.
        /// </summary>
        /// <param name="lastName">The last name.</param>
        /// <returns>Array of records.</returns>
        ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName);

        /// <summary>
        /// Finds the first name of the by.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        /// <returns>Array of records.</returns>
        ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName);
    }
}