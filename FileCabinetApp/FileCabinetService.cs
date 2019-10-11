using System;
using System.Collections.Generic;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        public int CreateRecord(string firstName, string lastName, char gender, DateTime dateOfBirth, decimal credit, short duration)
        {
            this.CheckInput(firstName, lastName, gender, dateOfBirth, credit, duration);
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

            return record.Id;
        }

        public FileCabinetRecord[] GetRecords()
        {
            if (this.list.Count == 0)
            {
                return Array.Empty<FileCabinetRecord>();
            }

            return this.list.ToArray();
        }

        public int GetStat()
        {
            return this.list.Count;
        }

        private void CheckInput(string firstName, string lastName, char gender, DateTime dateOfBirth, decimal credit, short duration)
        {
            if (firstName.Length < 2 || firstName.Length > 60)
            {
                throw new ArgumentException($"{nameof(firstName)}: First name length is upper than 60 or under than 2 symbols");
            }

            if (lastName.Length < 2 || lastName.Length > 60)
            {
                throw new ArgumentException($"{nameof(lastName)}: Last name length is upper than 60 or under than 2 symbols");
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

            if (gender != 'M' && gender != 'F')
            {
                throw new ArgumentException($"{nameof(gender)}: Indefinite gender");
            }
        }
    }
}