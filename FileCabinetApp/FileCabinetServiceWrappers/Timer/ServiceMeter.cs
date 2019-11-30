using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FileCabinetApp.Records;
using FileCabinetApp.Service;
using FileCabinetApp.Timer;

namespace FileCabinetApp.FileCabinetServiceWrappers.Timer
{
    /// <summary>
    /// ServiceMeter.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Service.IFileCabinetService" />
    public sealed class ServiceMeter : IFileCabinetService
    {
        private readonly IFileCabinetService service;
        private readonly ModelWriters modelWriter;
        private IStopWatcher stopwatch;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMeter"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="modelWriter">console writer.</param>
        public ServiceMeter(IFileCabinetService service, ModelWriters modelWriter)
        {
            this.service = service;
            this.modelWriter = modelWriter;
            this.Create();
        }

        /// <summary>
        /// Gets or sets the time in milliseconds.
        /// </summary>
        /// <value>
        /// The time in milliseconds.
        /// </value>
        private long TimeInMilliseconds { get; set; }

        /// <summary>
        /// Creates the record.
        /// </summary>
        /// <param name="record">record.</param>
        /// <returns>
        /// Id of created record.
        /// </returns>
        public int CreateRecord(FileCabinetRecord record)
        {
            return this.MethodMeter(this.service.CreateRecord, record, "insert");
        }

        /// <summary>
        /// Edits the record.
        /// </summary>
        /// <param name="fileCabinetRecord">The file cabinet record.</param>
        public void EditRecord(FileCabinetRecord fileCabinetRecord)
        {
            this.MethodMeter(this.service.EditRecord, fileCabinetRecord, "update");
        }

        /// <summary>
        /// Gets the stat.
        /// </summary>
        /// <returns>
        /// number of records.
        /// </returns>
        public (int, int) GetStat()
        {
            return this.MethodMeter(this.service.GetStat, "stat");
        }

        /// <summary>
        /// Gets the records.
        /// </summary>
        /// <returns>
        /// All records.
        /// </returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            return this.MethodMeter(this.service.GetRecords, "select");
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
            return this.MethodMeter(this.service.Restore, snapshot, "restore");
        }

        /// <summary>
        /// Removes the record.
        /// </summary>
        /// <param name="fileCabinetRecord">The file cabinet record.</param>
        /// <returns>record id.</returns>
        public int RemoveRecord(FileCabinetRecord fileCabinetRecord)
        {
            return this.MethodMeter(this.service.RemoveRecord, fileCabinetRecord, "delete");
        }

        /// <summary>
        /// Purges the deleted records.
        /// </summary>
        /// <returns>
        /// count of deleted records and count of all records.
        /// </returns>
        public (int, int) PurgeDeletedRecords()
        {
            return this.MethodMeter(this.service.PurgeDeletedRecords, "purge");
        }

        private void Create()
        {
            this.stopwatch = new StopWatcher();
        }

        private TResult MethodMeter<TSource, TResult>(Func<TSource, TResult> method, TSource record, string methodName)
        {
            this.stopwatch.StartTimer();
            TResult result = method.Invoke(record);
            this.stopwatch.StopTimer();
            this.TimeInMilliseconds = this.stopwatch.EllapsedMilliseconds;
            this.modelWriter.LineWriter.Invoke($"{methodName} method execution duration is {this.TimeInMilliseconds} ticks.");
            return result;
        }

        private void MethodMeter<TSource>(Action<TSource> method, TSource record, string methodName)
        {
            this.stopwatch.StartTimer();
            method.Invoke(record);
            this.stopwatch.StopTimer();
            this.TimeInMilliseconds = this.stopwatch.EllapsedMilliseconds;
            this.modelWriter.LineWriter.Invoke($"{methodName} method execution duration is {this.TimeInMilliseconds} ticks.");
        }

        private TResult MethodMeter<TResult>(Func<TResult> method, string methodName)
        {
            this.stopwatch.StartTimer();
            TResult result = method.Invoke();
            this.stopwatch.StopTimer();
            this.TimeInMilliseconds = this.stopwatch.EllapsedMilliseconds;
            this.modelWriter.LineWriter.Invoke($"{methodName} method execution duration is {this.TimeInMilliseconds} ticks.");
            return result;
        }
    }
}