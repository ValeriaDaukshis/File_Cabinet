using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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

        /// <summary>
        /// Creates the record.
        /// </summary>
        /// <param name="fileCabinetRecord">The file cabinet record.</param>
        /// <returns>id of created record.</returns>
        public int CreateRecord(FileCabinetRecord fileCabinetRecord)
        {
            fileCabinetRecord.Id = this.GenerateId(fileCabinetRecord);

            this.list.Add(fileCabinetRecord);
            this.AddValueToDictionary(fileCabinetRecord.FirstName, this.firstNameDictionary, fileCabinetRecord);
            this.AddValueToDictionary(fileCabinetRecord.LastName, this.lastNameDictionary, fileCabinetRecord);
            this.AddValueToDictionary(fileCabinetRecord.DateOfBirth, this.dateOfBirthDictionary, fileCabinetRecord);

            return fileCabinetRecord.Id;
        }

        public void RemoveRecord(FileCabinetRecord record)
        {
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
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            if (this.firstNameDictionary.Count == 0)
            {
                return new ReadOnlyCollection<FileCabinetRecord>(Array.Empty<FileCabinetRecord>());
            }

            if (this.firstNameDictionary.ContainsKey(firstName))
            {
                return this.firstNameDictionary[firstName].AsReadOnly();
            }

            return new ReadOnlyCollection<FileCabinetRecord>(Array.Empty<FileCabinetRecord>());
        }

        /// <summary>
        /// Finds the last name of the by.
        /// </summary>
        /// <param name="lastName">The last name.</param>
        /// <returns>Array of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            if (this.lastNameDictionary.Count == 0)
            {
                return new ReadOnlyCollection<FileCabinetRecord>(Array.Empty<FileCabinetRecord>());
            }

            if (this.lastNameDictionary.ContainsKey(lastName))
            {
                return this.lastNameDictionary[lastName].AsReadOnly();
            }

            return new ReadOnlyCollection<FileCabinetRecord>(Array.Empty<FileCabinetRecord>());
        }

        /// <summary>
        /// Finds the by date of birth.
        /// </summary>
        /// <param name="dateOfBirth">The date of birth.</param>
        /// <returns>Array of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            if (this.dateOfBirthDictionary.Count == 0)
            {
                return new ReadOnlyCollection<FileCabinetRecord>(Array.Empty<FileCabinetRecord>());
            }

            if (this.dateOfBirthDictionary.ContainsKey(dateOfBirth))
            {
                return this.dateOfBirthDictionary[dateOfBirth].AsReadOnly();
            }

            return new ReadOnlyCollection<FileCabinetRecord>(Array.Empty<FileCabinetRecord>());
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

        public int Restore(FileCabinetServiceSnapshot snapshot)
        {
            IList<FileCabinetRecord> readRecords = snapshot.ReadRecords;

            foreach (var record in readRecords)
            {
                var index = this.list.FindIndex(x => x.Id == record.Id);
                if (index != -1)
                {
                    this.list[index] = record;
                }
                else
                {
                    this.list.Add(record);
                }
            }

            return readRecords.Count;
        }

        /// <summary>
        /// Gets the stat.
        /// </summary>
        /// <returns>count of records.</returns>
        public int GetStat()
        {
            return this.list.Count;
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