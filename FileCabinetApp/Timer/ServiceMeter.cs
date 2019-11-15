using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using FileCabinetApp.Records;
using FileCabinetApp.Service;

namespace FileCabinetApp.Timer
{
    /// <summary>
    /// ServiceMeter.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Service.IFileCabinetService" />
    public class ServiceMeter : IFileCabinetService
    {
        private readonly IFileCabinetService service;
        private IStopWatcher stopwatch;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMeter"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        public ServiceMeter(IFileCabinetService service)
        {
            this.service = service;
            this.Create();
        }

        /// <summary>
        /// Gets or sets the time in milliseconds.
        /// </summary>
        /// <value>
        /// The time in milliseconds.
        /// </value>
        public long TimeInMilliseconds { get; set; }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        public void Create()
        {
            this.stopwatch = new StopWatcher();
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
            this.stopwatch.StartTimer();
            int recordId = this.service.CreateRecord(record);
            this.stopwatch.StopTimer();
            this.TimeInMilliseconds = this.stopwatch.EllapsedMilliseconds;
            Console.WriteLine($"Create method execution duration is {this.TimeInMilliseconds} ticks.");
            return recordId;
        }

        /// <summary>
        /// Edits the record.
        /// </summary>
        /// <param name="fileCabinetRecord">The file cabinet record.</param>
        public void EditRecord(FileCabinetRecord fileCabinetRecord)
        {
            this.stopwatch.StartTimer();
            this.service.EditRecord(fileCabinetRecord);
            this.stopwatch.StopTimer();
            this.TimeInMilliseconds = this.stopwatch.EllapsedMilliseconds;
            Console.WriteLine($"Edit method execution duration is {this.TimeInMilliseconds} ticks.");
        }

        /// <summary>
        /// Gets the stat.
        /// </summary>
        /// <returns>
        /// number of records.
        /// </returns>
        public (int, int) GetStat()
        {
            this.stopwatch.StartTimer();
            (int purgedRecords, int recordsCount) = this.service.GetStat();
            this.stopwatch.StopTimer();
            this.TimeInMilliseconds = this.stopwatch.EllapsedMilliseconds;
            Console.WriteLine($"Stat method execution duration is {this.TimeInMilliseconds} ticks.");
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
            this.stopwatch.StartTimer();
            ReadOnlyCollection<FileCabinetRecord> records = this.service.GetRecords();
            this.stopwatch.StopTimer();
            this.TimeInMilliseconds = this.stopwatch.EllapsedMilliseconds;
            Console.WriteLine($"Get method execution duration is {this.TimeInMilliseconds} ticks.");
            return records;
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
            this.stopwatch.StartTimer();
            IEnumerable<FileCabinetRecord> records = this.service.FindByDateOfBirth(dateOfBirth);
            this.stopwatch.StopTimer();
            this.TimeInMilliseconds = this.stopwatch.EllapsedMilliseconds;
            Console.WriteLine($"Find method execution duration is {this.TimeInMilliseconds} ticks.");
            return records;
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
            this.stopwatch.StartTimer();
            IEnumerable<FileCabinetRecord> records = this.service.FindByLastName(lastName);
            this.stopwatch.StopTimer();
            this.TimeInMilliseconds = this.stopwatch.EllapsedMilliseconds;
            Console.WriteLine($"Find method execution duration is {this.TimeInMilliseconds} ticks.");
            return records;
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
            this.stopwatch.StartTimer();
            IEnumerable<FileCabinetRecord> records = this.service.FindByFirstName(firstName);
            this.stopwatch.StopTimer();
            this.TimeInMilliseconds = this.stopwatch.EllapsedMilliseconds;
            Console.WriteLine($"Find method execution duration is {this.TimeInMilliseconds} ticks.");
            return records;
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
            this.stopwatch.StartTimer();
            int countOfRestored = this.service.Restore(snapshot);
            this.stopwatch.StopTimer();
            this.TimeInMilliseconds = this.stopwatch.EllapsedMilliseconds;
            Console.WriteLine($"Restore method execution duration is {this.TimeInMilliseconds} ticks.");
            return countOfRestored;
        }

        /// <summary>
        /// Removes the record.
        /// </summary>
        /// <param name="fileCabinetRecord">The file cabinet record.</param>
        /// <returns>record id.</returns>
        public int RemoveRecord(FileCabinetRecord fileCabinetRecord)
        {
            this.stopwatch.StartTimer();
            int id = this.service.RemoveRecord(fileCabinetRecord);
            this.stopwatch.StopTimer();
            this.TimeInMilliseconds = this.stopwatch.EllapsedMilliseconds;
            Console.WriteLine($"Remove method execution duration is {this.TimeInMilliseconds} ticks.");
            return id;
        }

        /// <summary>
        /// Purges the deleted records.
        /// </summary>
        /// <returns>
        /// count of deleted records and count of all records.
        /// </returns>
        public (int, int) PurgeDeletedRecords()
        {
            this.stopwatch.StartTimer();
            (int countOfDeletedRecords, int countOfRecords) = this.service.PurgeDeletedRecords();
            this.stopwatch.StopTimer();
            this.TimeInMilliseconds = this.stopwatch.EllapsedMilliseconds;
            Console.WriteLine($"Purge method execution duration is {this.TimeInMilliseconds} ticks.");
            return (countOfDeletedRecords, countOfRecords);
        }
    }
}