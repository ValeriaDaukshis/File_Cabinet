using System;
using System.Collections.Generic;
using FileCabinetApp.Validators;

namespace FileCabinetApp
{
    /// <summary>
    /// FileCabinetService.
    /// </summary>
    public abstract class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();
        private IRecordValidator validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetService"/> class.
        /// </summary>
        /// <param name="validator">validator.</param>
        protected FileCabinetService(IRecordValidator validator)
        {
            this.validator = validator;
        }

        /// <summary>
        /// Creates the record.
        /// </summary>
        /// <param name="fileCabinetRecord">The file cabinet record.</param>
        /// <returns>id of created record.</returns>
        public int CreateRecord(FileCabinetRecord fileCabinetRecord)
        {
            this.validator.ValidateParameters(fileCabinetRecord);
            var record = new FileCabinetRecord
                {
                    Id = this.list.Count + 1,
                    FirstName = fileCabinetRecord.FirstName,
                    LastName = fileCabinetRecord.LastName,
                    DateOfBirth = fileCabinetRecord.DateOfBirth,
                    Gender = fileCabinetRecord.Gender,
                    CreditSum = fileCabinetRecord.CreditSum,
                    Duration = fileCabinetRecord.Duration,
                };
            this.list.Add(record);
            this.AddValueToDictionary(fileCabinetRecord.FirstName, this.firstNameDictionary, record);
            this.AddValueToDictionary(fileCabinetRecord.LastName, this.lastNameDictionary, record);
            this.AddValueToDictionary(fileCabinetRecord.DateOfBirth, this.dateOfBirthDictionary, record);

            return record.Id;
        }

        /// <summary>
        /// Edits the record.
        /// </summary>
        /// <param name="fileCabinetRecord">The file cabinet record.</param>
        public void EditRecord(FileCabinetRecord fileCabinetRecord)
        {
            this.validator.ValidateParameters(fileCabinetRecord);
            FileCabinetRecord record = this.list.Find(x => x.Id == fileCabinetRecord.Id);

            if (fileCabinetRecord.FirstName != record.FirstName)
            {
                this.RemoveValueFromDictionary(record.FirstName, this.firstNameDictionary, record);
                this.AddValueToDictionary(fileCabinetRecord.FirstName, this.firstNameDictionary, record);
            }

            if (fileCabinetRecord.LastName != record.LastName)
            {
                this.RemoveValueFromDictionary(record.LastName, this.lastNameDictionary, record);
                this.AddValueToDictionary(fileCabinetRecord.LastName, this.lastNameDictionary, record);
            }

            if (fileCabinetRecord.DateOfBirth != record.DateOfBirth)
            {
                this.RemoveValueFromDictionary(record.DateOfBirth, this.dateOfBirthDictionary, record);
                this.AddValueToDictionary(fileCabinetRecord.DateOfBirth, this.dateOfBirthDictionary, record);
            }

            record.FirstName = fileCabinetRecord.FirstName;
            record.LastName = fileCabinetRecord.LastName;
            record.DateOfBirth = fileCabinetRecord.DateOfBirth;
            record.Gender = fileCabinetRecord.Gender;
            record.CreditSum = fileCabinetRecord.CreditSum;
            record.Duration = fileCabinetRecord.Duration;
        }

        /// <summary>
        /// Finds the first name of the by.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        /// <returns>Array of records.</returns>
        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            if (this.firstNameDictionary.Count == 0)
            {
                return Array.Empty<FileCabinetRecord>();
            }

            if (this.firstNameDictionary.ContainsKey(firstName))
            {
                return this.firstNameDictionary[firstName].ToArray();
            }

            return Array.Empty<FileCabinetRecord>();
        }

        /// <summary>
        /// Finds the last name of the by.
        /// </summary>
        /// <param name="lastName">The last name.</param>
        /// <returns>Array of records.</returns>
        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            if (this.lastNameDictionary.Count == 0)
            {
                return Array.Empty<FileCabinetRecord>();
            }

            if (this.lastNameDictionary.ContainsKey(lastName))
            {
                return this.lastNameDictionary[lastName].ToArray();
            }

            return Array.Empty<FileCabinetRecord>();
        }

        /// <summary>
        /// Finds the by date of birth.
        /// </summary>
        /// <param name="dateOfBirth">The date of birth.</param>
        /// <returns>Array of records.</returns>
        public FileCabinetRecord[] FindByDateOfBirth(DateTime dateOfBirth)
        {
            if (this.dateOfBirthDictionary.Count == 0)
            {
                return Array.Empty<FileCabinetRecord>();
            }

            if (this.dateOfBirthDictionary.ContainsKey(dateOfBirth))
            {
                return this.dateOfBirthDictionary[dateOfBirth].ToArray();
            }

            return Array.Empty<FileCabinetRecord>();
        }

        /// <summary>
        /// Gets the records.
        /// </summary>
        /// <returns>Array of records.</returns>
        public FileCabinetRecord[] GetRecords()
        {
            if (this.list.Count == 0)
            {
                return Array.Empty<FileCabinetRecord>();
            }

            return this.list.ToArray();
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
    }
}