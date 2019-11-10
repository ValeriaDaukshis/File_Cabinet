using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using FileCabinetApp.Records;
using FileCabinetApp.Service;

namespace FileCabinetApp.Logger
{
    /// <summary>
    /// ServiceLogger.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Service.IFileCabinetService" />
    public class ServiceLogger : IFileCabinetService, IDisposable
    {
        private readonly IFileCabinetService service;
        private readonly string path;
        private TextWriter writer;
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLogger"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="path">The path.</param>
        public ServiceLogger(IFileCabinetService service, string path)
        {
            this.service = service;
            this.path = path;
            this.Create();
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        public void Create()
        {
            this.writer = new StreamWriter(this.path);
        }

        /// <summary>
        /// Creates the record.
        /// </summary>
        /// <param name="record">record.</param>
        /// <returns>
        /// Id of created record.
        /// </returns>
        public int CreateRecord(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), $"{nameof(record)} is null");
            }

            this.writer.WriteLine(this.CreateTextWithParameters("create", record));
            int id = this.service.CreateRecord(record);
            this.writer.WriteLine($"create() returned '{id}'");
            this.writer.Flush();
            return id;
        }

        /// <summary>
        /// Edits the record.
        /// </summary>
        /// <param name="record">The record.</param>
        public void EditRecord(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), $"{nameof(record)} is null");
            }

            this.writer.WriteLine(this.CreateTextWithParameters("edit", record));
            this.service.EditRecord(record);
            this.writer.Flush();
        }

        /// <summary>
        /// Gets the stat.
        /// </summary>
        /// <returns>
        /// number of records.
        /// </returns>
        public (int, int) GetStat()
        {
            this.writer.WriteLine($"{CreateText("Stat")}");
            (int purgedRecords, int recordsCount) = this.service.GetStat();
            this.writer.WriteLine($"Stat() returned '{purgedRecords}', '{recordsCount}'");
            this.writer.Flush();
            return (purgedRecords, recordsCount);
        }

        /// <summary>
        /// Gets the records.
        /// </summary>
        /// <returns>
        /// All records.
        /// </returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            this.writer.WriteLine($"{CreateText("Get")}");
            ReadOnlyCollection<FileCabinetRecord> collection = this.service.GetRecords();
            this.writer.WriteLine($"Get() returned '{collection}'");
            this.writer.Flush();
            return collection;
        }

        /// <summary>
        /// Finds the by date of birth.
        /// </summary>
        /// <param name="dateOfBirth">The date of birth.</param>
        /// <returns>
        /// Array of records.
        /// </returns>
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            this.writer.WriteLine($"{CreateText("FindByDateOfBirth")} with firstName = '{dateOfBirth}'");
            IEnumerable<FileCabinetRecord> collection = this.service.FindByDateOfBirth(dateOfBirth);
            this.writer.WriteLine($"FindByDateOfBirth() returned '{collection}'");
            this.writer.Flush();
            return collection;
        }

        /// <summary>
        /// Finds the last name of the by.
        /// </summary>
        /// <param name="lastName">The last name.</param>
        /// <returns>
        /// Array of records.
        /// </returns>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            this.writer.WriteLine($"{CreateText("FindByLastName")} with firstName = '{lastName}'");
            IEnumerable<FileCabinetRecord> collection = this.service.FindByLastName(lastName);
            this.writer.WriteLine($"FindByLastName() returned '{collection}'");
            return collection;
        }

        /// <summary>
        /// Finds the first name of the by.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        /// <returns>
        /// Array of records.
        /// </returns>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            this.writer.WriteLine($"{CreateText("FindByFirstName")} with firstName = '{firstName}'");
            IEnumerable<FileCabinetRecord> collection = this.service.FindByFirstName(firstName);
            this.writer.WriteLine($"FindByFirstName() returned '{collection}'");
            this.writer.Flush();
            return collection;
        }

        /// <summary>
        /// Makes the snapshot.
        /// </summary>
        /// <returns>
        /// FileCabinetServiceSnapshot.
        /// </returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return this.service.MakeSnapshot();
        }

        /// <summary>
        /// Restores the specified snapshot.
        /// </summary>
        /// <param name="snapshot">The snapshot.</param>
        /// <returns>
        /// count of restored records.
        /// </returns>
        public int Restore(FileCabinetServiceSnapshot snapshot)
        {
            this.writer.WriteLine($"{CreateText("Restore")} with snapshot = '{snapshot}'");
            int count = this.service.Restore(snapshot);
            this.writer.WriteLine($"Restore() returned '{count}'");
            this.writer.Flush();
            return count;
        }

        /// <summary>
        /// Removes the record.
        /// </summary>
        /// <param name="record">The record.</param>
        public void RemoveRecord(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), $"{nameof(record)} is null");
            }

            this.writer.WriteLine(this.CreateTextWithParameters("delete", record));
            this.service.RemoveRecord(record);
            this.writer.Flush();
        }

        /// <summary>
        /// Purges the deleted records.
        /// </summary>
        /// <returns>
        /// count of deleted records and count of all records.
        /// </returns>
        public (int, int) PurgeDeletedRecords()
        {
            this.writer.WriteLine(CreateText("purge"));
            (int countOfDeletedRecords, int countOfRecords) = this.service.PurgeDeletedRecords();
            this.writer.WriteLine($"create() returned '{countOfDeletedRecords}', '{countOfRecords}'");
            this.writer.Flush();
            return (countOfDeletedRecords, countOfRecords);
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private static string CreateText(string method)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(DateTime.Now);
            builder.Append($" Calling {method}()");
            return builder.ToString();
        }

        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.writer.Dispose();
            }

            this.disposed = true;
        }

        private string CreateTextWithParameters(string method, FileCabinetRecord record)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(CreateText(method));
            builder.Append($" with firstName = '{record.FirstName}' ");
            builder.Append($"lastName = '{record.LastName}' ");
            builder.Append($"Gender = '{record.Gender}' ");
            builder.Append($"DateOfBirth = '{record.DateOfBirth:mm/dd/yyyy}' ");
            builder.Append($"CreditSum = '{record.CreditSum}' ");
            builder.Append($"Duration = '{record.Duration}' ");
            return builder.ToString();
        }
    }
}