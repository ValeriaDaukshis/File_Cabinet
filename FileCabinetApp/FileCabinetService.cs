using System;
using System.Collections.Generic;

namespace FileCabinetApp
{
    /// <summary>
    /// FileCabinetService.
    /// </summary>
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();

        /// <summary>
        /// Creates the record.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="gender">The gender.</param>
        /// <param name="dateOfBirth">The date of birth.</param>
        /// <param name="credit">The credit.</param>
        /// <param name="duration">The duration.</param>
        /// <returns>record Id.</returns>
        public int CreateRecord(string firstName, string lastName, char gender, DateTime dateOfBirth, decimal credit, short duration)
        {
            CheckInput(firstName, lastName, gender, dateOfBirth, credit, duration);

            var record = new FileCabinetRecord
                {
                    Id = this.list.Count + 1,
                    FirstName = firstName,
                    LastName = lastName,
                    DateOfBirth = dateOfBirth,
                    Gender = gender,
                    CreditSum = credit,
                    Duration = duration,
                };
            this.list.Add(record);
            this.AddValueToDictionary(firstName, this.firstNameDictionary, record);
            this.AddValueToDictionary(lastName, this.lastNameDictionary, record);
            this.AddValueToDictionary(dateOfBirth, this.dateOfBirthDictionary, record);

            return record.Id;
        }

        /// <summary>
        /// Edits the record.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="gender">The gender.</param>
        /// <param name="dateOfBirth">The date of birth.</param>
        /// <param name="credit">The credit.</param>
        /// <param name="duration">The duration.</param>
        public void EditRecord(int id, string firstName, string lastName, char gender, DateTime dateOfBirth, decimal credit, short duration)
        {
            CheckInput(firstName, lastName, gender, dateOfBirth, credit, duration);
            FileCabinetRecord record = this.list.Find(x => x.Id == id);

            if (firstName != record.FirstName)
            {
                this.RemoveValueFromDictionary(record.FirstName, this.firstNameDictionary, record);
                this.AddValueToDictionary(firstName, this.firstNameDictionary, record);
            }

            if (lastName != record.LastName)
            {
                this.RemoveValueFromDictionary(record.LastName, this.lastNameDictionary, record);
                this.AddValueToDictionary(lastName, this.lastNameDictionary, record);
            }

            if (dateOfBirth != record.DateOfBirth)
            {
                this.RemoveValueFromDictionary(record.DateOfBirth, this.dateOfBirthDictionary, record);
                this.AddValueToDictionary(dateOfBirth, this.dateOfBirthDictionary, record);
            }

            record.FirstName = firstName;
            record.LastName = lastName;
            record.DateOfBirth = dateOfBirth;
            record.Gender = gender;
            record.CreditSum = credit;
            record.Duration = duration;
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

        private static void CheckInput(string firstName, string lastName, char gender, DateTime dateOfBirth, decimal credit, short duration)
        {
            if (string.IsNullOrEmpty(firstName))
            {
                throw new ArgumentNullException($"{nameof(firstName)}: First name is null");
            }

            if (string.IsNullOrEmpty(lastName))
            {
                throw new ArgumentNullException($"{nameof(lastName)}: Last name is null");
            }

            if (firstName.Length < 2 || firstName.Length > 60)
            {
                throw new ArgumentException($"{nameof(firstName)}: First name length is upper than 60 or under than 2 symbols");
            }

            if (lastName.Length < 2 || lastName.Length > 60)
            {
                throw new ArgumentException($"{nameof(lastName)}: Last name length is upper than 60 or under than 2 symbols");
            }

            if (gender != 'M' && gender != 'F')
            {
                throw new ArgumentException($"{nameof(gender)}: Indefinite gender");
            }

            if (dateOfBirth > DateTime.Now)
            {
                throw new ArgumentException($"{nameof(dateOfBirth)}: Date of birth is upper than today's date");
            }

            if (dateOfBirth < new DateTime(1950, 01, 01))
            {
                throw new ArgumentException($"{nameof(dateOfBirth)}: Date of birth is under than 01-Jan-1950");
            }

            if (credit > 5000 || credit < 10)
            {
                throw new ArgumentException($"{nameof(credit)}: Credit sum is upper than 5000 or under than 10 BYN");
            }

            if (duration > 120 || duration < 6)
            {
                throw new ArgumentException($"{nameof(duration)}: Credit duration is upper than 120 or under than 6 weeks");
            }
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