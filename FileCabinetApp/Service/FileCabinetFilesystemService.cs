using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using FileCabinetApp.ExceptionClasses;

namespace FileCabinetApp.Service
{
    /// <summary>
    /// FileCabinetFilesystemService.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Service.IFileCabinetService" />
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private const int SizeOfStringRecord = 120;
        private const long RecordSize = (sizeof(short) * 2) + (SizeOfStringRecord * 2) + sizeof(char) + (sizeof(int) * 4) + sizeof(decimal);
        private readonly FileStream fileStream;
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();
        private readonly Dictionary<int, long> recordIndexPosition = new Dictionary<int, long>();
        private int countOfRecords;

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
            fileCabinetRecord.Id = this.GenerateId(fileCabinetRecord);
            this.countOfRecords++;
            BinaryWriter writer = new BinaryWriter(this.fileStream);
            this.FileWriter(fileCabinetRecord, writer);
            writer.Flush();

            this.recordIndexPosition.Add(fileCabinetRecord.Id, RecordSize * (this.countOfRecords - 1));
            this.AddValueToDictionary(fileCabinetRecord.FirstName, this.firstNameDictionary, fileCabinetRecord);
            this.AddValueToDictionary(fileCabinetRecord.LastName, this.lastNameDictionary, fileCabinetRecord);
            this.AddValueToDictionary(fileCabinetRecord.DateOfBirth, this.dateOfBirthDictionary, fileCabinetRecord);

            return fileCabinetRecord.Id;
        }

        /// <summary>
        /// Edits the record.
        /// </summary>
        /// <param name="fileCabinetRecord">The file cabinet record.</param>
        public void EditRecord(FileCabinetRecord fileCabinetRecord)
        {
            BinaryReader reader = new BinaryReader(this.fileStream);
            if (!this.recordIndexPosition.ContainsKey(fileCabinetRecord.Id))
            {
                throw new FileRecordNotFoundException(fileCabinetRecord.Id);
            }

            long position = this.recordIndexPosition[fileCabinetRecord.Id];
            FileCabinetRecord oldRecord = this.FileReader(reader, position);
            BinaryWriter writer = new BinaryWriter(this.fileStream);

            writer.BaseStream.Position = position + sizeof(short) + sizeof(int);
            if (fileCabinetRecord.FirstName != oldRecord.FirstName)
            {
                var buffer = Encoding.Unicode.GetBytes(this.CreateEmptyString(fileCabinetRecord.FirstName, 60));
                writer.Write(buffer);
                this.RemoveValueFromDictionary(oldRecord.FirstName, this.firstNameDictionary, oldRecord);
                this.AddValueToDictionary(fileCabinetRecord.FirstName, this.firstNameDictionary, fileCabinetRecord);
            }
            else
            {
                writer.BaseStream.Position += 120;
            }

            if (fileCabinetRecord.LastName != oldRecord.LastName)
            {
                var buffer = Encoding.Unicode.GetBytes(this.CreateEmptyString(fileCabinetRecord.LastName, 60));
                writer.Write(buffer);
                this.RemoveValueFromDictionary(oldRecord.LastName, this.lastNameDictionary, oldRecord);
                this.AddValueToDictionary(fileCabinetRecord.LastName, this.lastNameDictionary, fileCabinetRecord);
            }
            else
            {
                writer.BaseStream.Position += 120;
            }

            if (fileCabinetRecord.Gender != oldRecord.Gender)
            {
                var buffer = Encoding.Unicode.GetBytes(this.CreateEmptyString(fileCabinetRecord.Gender.ToString(), 1));
                writer.Write(buffer);
            }
            else
            {
                writer.BaseStream.Position += 2;
            }

            if (fileCabinetRecord.DateOfBirth != oldRecord.DateOfBirth)
            {
                writer.Write(fileCabinetRecord.DateOfBirth.Year);
                writer.Write(fileCabinetRecord.DateOfBirth.Month);
                writer.Write(fileCabinetRecord.DateOfBirth.Day);

                this.RemoveValueFromDictionary(oldRecord.DateOfBirth, this.dateOfBirthDictionary, oldRecord);
                this.AddValueToDictionary(fileCabinetRecord.DateOfBirth, this.dateOfBirthDictionary, fileCabinetRecord);
            }
            else
            {
                writer.BaseStream.Position += 12;
            }

            if (fileCabinetRecord.CreditSum != oldRecord.CreditSum)
            {
                writer.Write(fileCabinetRecord.CreditSum);
            }
            else
            {
                writer.BaseStream.Position += 16;
            }

            if (fileCabinetRecord.Duration != oldRecord.Duration)
            {
                writer.Write(fileCabinetRecord.Duration);
            }
        }

        /// <summary>
        /// Gets the stat.
        /// </summary>
        /// <returns>
        /// number of records.
        /// </returns>
        public int GetStat()
        {
            return this.countOfRecords;
        }

        /// <summary>
        /// Gets the records.
        /// </summary>
        /// <returns>
        /// All records.
        /// </returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            List<FileCabinetRecord> list = new List<FileCabinetRecord>();
            BinaryReader reader = new BinaryReader(this.fileStream);

            reader.BaseStream.Position = 0;
            long position = reader.BaseStream.Position;
            for (int i = 0; i < this.countOfRecords; i++)
            {
                list.Add(this.FileReader(reader, position));
                position += RecordSize;
            }

            return list.AsReadOnly();
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
        /// Finds the last name of the by.
        /// </summary>
        /// <param name="lastName">The last name.</param>
        /// <returns>
        /// Array of records.
        /// </returns>
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
        /// Finds the first name of the by.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        /// <returns>
        /// Array of records.
        /// </returns>
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
        /// Makes the snapshot.
        /// </summary>
        /// <returns>
        /// FileCabinetServiceSnapshot.
        /// </returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.GetRecords().ToArray());
        }

        public int Restore(FileCabinetServiceSnapshot snapshot)
        {
            IList<FileCabinetRecord> readRecords = snapshot.ReadRecords;

            foreach (var record in readRecords)
            {
                if (this.recordIndexPosition.ContainsKey(record.Id))
                {
                    this.EditRecord(record);
                }
                else
                {
                    this.CreateRecord(record);
                }
            }

            return readRecords.Count;
        }

        private string CreateEmptyString(string s, int capacity)
        {
            StringBuilder builder = new StringBuilder(capacity);
            builder.Append(s);
            for (int i = s.Length; i < builder.Capacity; i++)
            {
                builder.Append(default(char));
            }

            return builder.ToString();
        }

        private FileCabinetRecord FileReader(BinaryReader reader, long pointer)
        {
            reader.BaseStream.Position = pointer;
            short reserved = reader.ReadInt16();
            int id = reader.ReadInt32();
            string firstName = Encoding.Unicode.GetString(reader.ReadBytes(SizeOfStringRecord), 0, SizeOfStringRecord);
            string lastName = Encoding.Unicode.GetString(reader.ReadBytes(SizeOfStringRecord), 0, SizeOfStringRecord);
            char gender = BitConverter.ToChar(reader.ReadBytes(2), 0);
            int year = BitConverter.ToInt32(reader.ReadBytes(4), 0);
            int month = BitConverter.ToInt32(reader.ReadBytes(4), 0);
            int day = BitConverter.ToInt32(reader.ReadBytes(4), 0);
            DateTime dateOfBirth = new DateTime(year, month, day);
            decimal credit = reader.ReadDecimal();
            short duration = reader.ReadInt16();
            var fileCabinetRecord = new FileCabinetRecord(firstName, lastName, gender, dateOfBirth, credit, duration);
            fileCabinetRecord.Id = id;

            return fileCabinetRecord;
        }

        private void FileWriter(FileCabinetRecord fileCabinetRecord, BinaryWriter writer)
        {
            writer.Write(default(short));
            writer.Write(fileCabinetRecord.Id);

            var buffer = Encoding.Unicode.GetBytes(this.CreateEmptyString(fileCabinetRecord.FirstName, 60));
            writer.Write(buffer);

            buffer = Encoding.Unicode.GetBytes(this.CreateEmptyString(fileCabinetRecord.LastName, 60));
            writer.Write(buffer);

            buffer = Encoding.Unicode.GetBytes(this.CreateEmptyString(fileCabinetRecord.Gender.ToString(), 1));
            writer.Write(buffer);

            writer.Write(fileCabinetRecord.DateOfBirth.Year);
            writer.Write(fileCabinetRecord.DateOfBirth.Month);
            writer.Write(fileCabinetRecord.DateOfBirth.Day);
            writer.Write(fileCabinetRecord.CreditSum);
            writer.Write(fileCabinetRecord.Duration);
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

            if (this.recordIndexPosition.Count > 0)
            {
                maxId = this.recordIndexPosition.Keys.Max(x => x);
            }

            return maxId + 1;
        }
    }
}