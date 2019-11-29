using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using FileCabinetApp.Records;
using FileCabinetApp.Service;

namespace FileCabinetApp.FileCabinetServiceWrappers.Logger
{
    /// <summary>
    /// ServiceLogger.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Service.IFileCabinetService" />
    public sealed class ServiceLogger : IFileCabinetService, IDisposable
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

            return this.MethodLogger(this.service.CreateRecord, record, "create");
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

            this.MethodLogger(this.service.EditRecord, record, "update");
        }

        /// <summary>
        /// Gets the stat.
        /// </summary>
        /// <returns>
        /// number of records.
        /// </returns>
        public (int, int) GetStat()
        {
            return this.MethodLogger(this.service.GetStat, "Stat");
        }

        /// <summary>
        /// Gets the records.
        /// </summary>
        /// <returns>
        /// All records.
        /// </returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            return this.MethodLogger(this.service.GetRecords, "Select");
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
            return this.MethodLogger(this.service.Restore, snapshot, "restore");
        }

        /// <summary>
        /// Removes the record.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns>record id.</returns>
        public int RemoveRecord(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), $"{nameof(record)} is null");
            }

            return this.MethodLogger(this.service.RemoveRecord, record, "delete");
        }

        /// <summary>
        /// Purges the deleted records.
        /// </summary>
        /// <returns>
        /// count of deleted records and count of all records.
        /// </returns>
        public (int, int) PurgeDeletedRecords()
        {
            return this.MethodLogger(this.service.PurgeDeletedRecords, "purge");
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

        private void Create()
        {
            this.writer = new StreamWriter(this.path);
        }

        private TResult MethodLogger<TSource, TResult>(Func<TSource, TResult> method, TSource record, string methodName)
        {
            this.writer.WriteLine(CreateText(methodName));
            TResult result = method.Invoke(record);
            this.writer.WriteLine($"{methodName}() returned '{result}'");
            this.writer.Flush();
            return result;
        }

        private void MethodLogger<TSource>(Action<TSource> method, TSource record, string methodName)
        {
            this.writer.WriteLine(CreateText(methodName));
            method.Invoke(record);
            this.writer.Flush();
        }

        private TResult MethodLogger<TResult>(Func<TResult> method, string methodName)
        {
            this.writer.WriteLine(CreateText(methodName));
            TResult result = method.Invoke();
            this.writer.WriteLine($"{methodName}() returned '{result}'");
            this.writer.Flush();
            return result;
        }

        private (TResult, TResult) MethodLogger<TResult>(Func<(TResult, TResult)> method, string methodName)
        {
            this.writer.WriteLine(CreateText(methodName));
            (TResult result1, TResult result2) = method.Invoke();
            this.writer.WriteLine($"{methodName}() returned '{result1}', '{result2}'");
            this.writer.Flush();
            return (result1, result2);
        }
    }
}