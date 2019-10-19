using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace FileCabinetApp.Service
{
    /// <summary>
    /// FileCabinetFilesystemService.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Service.IFileCabinetService" />
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private readonly FileStream fileStream;
        private int countOfRecords;
        private readonly string path = @"C:\Users\dauks\Dop Task Epam\example.txt";

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="fileStream">The file stream.</param>
        public FileCabinetFilesystemService(FileStream fileStream)
        {
            this.fileStream = fileStream;
            this.countOfRecords = 0;
        }

        /// <summary>
        /// Creates the record.
        /// </summary>
        /// <param name="fileCabinetRecord">The file cabinet record.</param>
        /// <returns>
        /// Id of created record.
        /// </returns>
        public int CreateRecord(FileCabinetRecord fileCabinetRecord)
        {
            fileCabinetRecord.Id = this.countOfRecords + 1;
            this.countOfRecords++;
            using (BinaryWriter writer = new BinaryWriter(this.fileStream))
            {
                writer.Write(default(short));
                writer.Write(fileCabinetRecord.Id);
                writer.Write(Encoding.UTF8.GetBytes(fileCabinetRecord.FirstName));
                writer.Write(Encoding.UTF8.GetBytes(fileCabinetRecord.LastName));
                writer.Write(fileCabinetRecord.DateOfBirth.Year);
                writer.Write(fileCabinetRecord.DateOfBirth.Month);
                writer.Write(fileCabinetRecord.DateOfBirth.Day);
                writer.Write(fileCabinetRecord.Gender);
                writer.Write(fileCabinetRecord.CreditSum);
                writer.Write(fileCabinetRecord.Duration);
            }

            return fileCabinetRecord.Id;
        }

        /// <summary>
        /// Edits the record.
        /// </summary>
        /// <param name="fileCabinetRecord">The file cabinet record.</param>
        public void EditRecord(FileCabinetRecord fileCabinetRecord)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the stat.
        /// </summary>
        /// <returns>
        /// number of records.
        /// </returns>
        public int GetStat()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the records.
        /// </summary>
        /// <returns>
        /// All records.
        /// </returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds the by date of birth.
        /// </summary>
        /// <param name="dateOfBirth">The date of birth.</param>
        /// <returns>
        /// Array of records.
        /// </returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds the last name of the by.
        /// </summary>
        /// <param name="lastName">The last name.</param>
        /// <returns>
        /// Array of records.
        /// </returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds the first name of the by.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        /// <returns>
        /// Array of records.
        /// </returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Makes the snapshot.
        /// </summary>
        /// <returns>
        /// FileCabinetServiceSnapshot.
        /// </returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            throw new NotImplementedException();
        }
    }
}