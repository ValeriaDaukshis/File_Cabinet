using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FileCabinetApp.Records;
using FileCabinetApp.Validators;

namespace FileCabinetApp.Service
{
    /// <summary>
    /// FileCabinetService.
    /// </summary>
    public class FileCabinetMemoryService : IFileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();
        private readonly IRecordValidator validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.
        /// </summary>
        /// <param name="validator">the validator.</param>
        public FileCabinetMemoryService(IRecordValidator validator)
        {
            this.validator = validator;
        }

        /// <summary>
        /// Creates the record.
        /// </summary>
        /// <param name="record">The file cabinet record.</param>
        /// <returns>id of created record.</returns>
        public int CreateRecord(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), $"{nameof(record)} is null");
            }

            this.validator.ValidateParameters(record);
            record.Id = this.GenerateId(record);
            this.list.Add(record);
            this.AddValueToDictionary(record.FirstName, this.firstNameDictionary, record);
            this.AddValueToDictionary(record.LastName, this.lastNameDictionary, record);
            this.AddValueToDictionary(record.DateOfBirth, this.dateOfBirthDictionary, record);

            return record.Id;
        }

        /// <summary>
        /// RemoveRecord.
        /// </summary>
        /// <param name="record">record.</param>
        public void RemoveRecord(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), $"{nameof(record)} is null");
            }

            int position = this.list.FindIndex(x => x.Id == record.Id);
            this.list.RemoveAt(position);

            if (this.firstNameDictionary.ContainsKey(record.FirstName))
            {
                this.RemoveValueFromDictionary(record.FirstName, this.firstNameDictionary, record);
            }

            if (this.lastNameDictionary.ContainsKey(record.LastName))
            {
                this.RemoveValueFromDictionary(record.LastName, this.lastNameDictionary, record);
            }

            if (this.dateOfBirthDictionary.ContainsKey(record.DateOfBirth))
            {
                this.RemoveValueFromDictionary(record.DateOfBirth, this.dateOfBirthDictionary, record);
            }
        }

        /// <summary>
        /// Purges the deleted records.
        /// </summary>
        /// <returns>count of deleted records and count of all records.</returns>
        public (int, int) PurgeDeletedRecords()
        {
            int countOfRecords = this.list.Count;
            return (0, countOfRecords);
        }

        /// <summary>
        /// Edits the record.
        /// </summary>
        /// <param name="fileCabinetRecord">The file cabinet record.</param>
        public void EditRecord(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord is null)
            {
                throw new ArgumentNullException(nameof(fileCabinetRecord), $"{nameof(fileCabinetRecord)} is null");
            }

            FileCabinetRecord record = this.list.Find(x => x.Id == fileCabinetRecord.Id);

            if (fileCabinetRecord.FirstName != record.FirstName)
            {
                this.RemoveValueFromDictionary(record.FirstName, this.firstNameDictionary, record);
                this.AddValueToDictionary(fileCabinetRecord.FirstName, this.firstNameDictionary, fileCabinetRecord);
            }

            if (fileCabinetRecord.LastName != record.LastName)
            {
                this.RemoveValueFromDictionary(record.LastName, this.lastNameDictionary, record);
                this.AddValueToDictionary(fileCabinetRecord.LastName, this.lastNameDictionary, fileCabinetRecord);
            }

            if (fileCabinetRecord.DateOfBirth != record.DateOfBirth)
            {
                this.RemoveValueFromDictionary(record.DateOfBirth, this.dateOfBirthDictionary, record);
                this.AddValueToDictionary(fileCabinetRecord.DateOfBirth, this.dateOfBirthDictionary, fileCabinetRecord);
            }
        }

        /// <summary>
        /// Finds the first name of the by.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        /// <returns>Array of records.</returns>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            if (this.firstNameDictionary.ContainsKey(firstName))
            {
                foreach (var element in this.firstNameDictionary[firstName])
                {
                    yield return element;
                }
            }

            foreach (var fileCabinetRecord in Array.Empty<FileCabinetRecord>())
            {
                yield return fileCabinetRecord;
            }
        }

        /// <summary>
        /// Finds the last name of the by.
        /// </summary>
        /// <param name="lastName">The last name.</param>
        /// <returns>Array of records.</returns>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            if (this.lastNameDictionary.ContainsKey(lastName))
            {
                foreach (var element in this.lastNameDictionary[lastName])
                {
                    yield return element;
                }
            }

            foreach (var fileCabinetRecord in Array.Empty<FileCabinetRecord>())
            {
                yield return fileCabinetRecord;
            }
        }

        /// <summary>
        /// Finds the by date of birth.
        /// </summary>
        /// <param name="dateOfBirth">The date of birth.</param>
        /// <returns>Array of records.</returns>
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            if (this.dateOfBirthDictionary.ContainsKey(dateOfBirth))
            {
                foreach (var element in this.dateOfBirthDictionary[dateOfBirth])
                {
                    yield return element;
                }
            }

            foreach (var fileCabinetRecord in Array.Empty<FileCabinetRecord>())
            {
                yield return fileCabinetRecord;
            }
        }

        /// <summary>
        /// Gets the records.
        /// </summary>
        /// <returns>Array of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            if (this.list.Count == 0)
            {
                return new ReadOnlyCollection<FileCabinetRecord>(Array.Empty<FileCabinetRecord>());
            }

            return this.list.AsReadOnly();
        }

        /// <summary>
        /// Makes the snapshot.
        /// </summary>
        /// <returns>
        /// FileCabinetServiceSnapshot.
        /// </returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.list.ToArray());
        }

        /// <summary>
        /// Restore.
        /// </summary>
        /// <param name="snapshot">snapshot.</param>
        /// <returns>records count.</returns>
        public int Restore(FileCabinetServiceSnapshot snapshot)
        {
            if (snapshot is null)
            {
                throw new ArgumentNullException(nameof(snapshot), $"{nameof(snapshot)} is null");
            }

            IList<FileCabinetRecord> readRecords = snapshot.ReadRecords;

            foreach (var record in readRecords)
            {
                var index = this.list.FindIndex(x => x.Id == record.Id);
                if (index != -1)
                {
                    this.list[index] = record;
                    this.EditRecord(record);
                }
                else
                {
                    this.list.Add(record);
                    this.AddValueToDictionary(record.FirstName, this.firstNameDictionary, record);
                    this.AddValueToDictionary(record.LastName, this.lastNameDictionary, record);
                    this.AddValueToDictionary(record.DateOfBirth, this.dateOfBirthDictionary, record);
                }
            }

            return readRecords.Count;
        }

        /// <summary>
        /// Gets the stat.
        /// </summary>
        /// <returns>count of records.</returns>
        public (int, int) GetStat()
        {
            return (0, this.list.Count);
        }

        private void AddValueToDictionary<T>(T value, Dictionary<T, List<FileCabinetRecord>> dictionary, FileCabinetRecord record)
        {
            if (dictionary.ContainsKey(value))
            {
                dictionary[value].Add(record);
            }
            else
            {
                dictionary.Add(value, new List<FileCabinetRecord>());
                dictionary[value].Add(record);
            }
        }

        private void RemoveValueFromDictionary<T>(T value, Dictionary<T, List<FileCabinetRecord>> dictionary, FileCabinetRecord record)
        {
            if (dictionary.ContainsKey(value))
            {
                dictionary[value].Remove(record);
            }
        }

        private int GenerateId(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord.Id != 0)
            {
                return fileCabinetRecord.Id;
            }

            var maxId = 0;

            if (this.list.Count > 0)
            {
                maxId = this.list.Max(x => x.Id);
            }

            return maxId + 1;
        }
    }
}