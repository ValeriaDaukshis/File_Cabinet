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
        private const long RecordSize = 278;
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
            long position = RecordSize * (this.countOfRecords - 1);
            this.FileWriter(fileCabinetRecord, writer, position);
            writer.Flush();

            this.recordIndexPosition.Add(fileCabinetRecord.Id, position);
            this.AddValueToDictionary(fileCabinetRecord.FirstName, this.firstNameDictionary, fileCabinetRecord);
            this.AddValueToDictionary(fileCabinetRecord.LastName, this.lastNameDictionary, fileCabinetRecord);
            this.AddValueToDictionary(fileCabinetRecord.DateOfBirth, this.dateOfBirthDictionary, fileCabinetRecord);

            return fileCabinetRecord.Id;
        }

        /// <summary>
        /// Removes the record.
        /// </summary>
        /// <param name="record">The record.</param>
        public void RemoveRecord(FileCabinetRecord record)
        {
            long position = this.recordIndexPosition[record.Id];
            BinaryWriter writer = new BinaryWriter(this.fileStream);
            writer.BaseStream.Position = position;
            short reserved = 1;
            writer.Write(reserved);
            writer.Flush();

            this.recordIndexPosition.Remove(record.Id);
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
            Queue<long> rewriteRecord = new Queue<long>();
            BinaryWriter writer = new BinaryWriter(this.fileStream);
            BinaryReader reader = new BinaryReader(this.fileStream);

            this.recordIndexPosition.Clear();
            writer.BaseStream.Position = 0;
            reader.BaseStream.Position = 0;

            long readerPosition = 0;
            long sizeOfFile = 0;
            int countOfDeletedRecords = 0;

            while (readerPosition != reader.BaseStream.Length)
            {
                rewriteRecord.Enqueue(readerPosition);
                FileCabinetRecord record = this.FileReader(reader, readerPosition);
                if (record != null)
                {
                    long recordPosition = rewriteRecord.Dequeue();
                    this.FileWriter(record, writer, recordPosition);
                    this.recordIndexPosition.Add(record.Id, recordPosition);
                    sizeOfFile += RecordSize;
                }
                else
                {
                    countOfDeletedRecords++;
                }

                readerPosition += RecordSize;
            }

            this.fileStream.SetLength(sizeOfFile);
            int countOfAllRecords = this.countOfRecords;
            this.countOfRecords -= countOfDeletedRecords;
            return (countOfDeletedRecords, countOfAllRecords);
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
        
        public (int, int) GetStat()
        {
            int reservedRecords = this.countOfRecords - this.recordIndexPosition.Count;
            return (reservedRecords, this.countOfRecords);
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
                FileCabinetRecord record = this.FileReader(reader, position);
                if (record != null)
                {
                    list.Add(record);
                }

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

        /// <summary>
        /// Restores the specified snapshot.
        /// </summary>
        /// <param name="snapshot">The snapshot.</param>
        /// <returns>
        /// count of restored records.
        /// </returns>
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
            if (reserved == 1)
            {
                return null;
            }

            int id = reader.ReadInt32();
            string firstName = Encoding.Unicode.GetString(reader.ReadBytes(SizeOfStringRecord), 0, SizeOfStringRecord);
            firstName = firstName.Replace("\0", string.Empty, StringComparison.InvariantCulture);

            string lastName = Encoding.Unicode.GetString(reader.ReadBytes(SizeOfStringRecord), 0, SizeOfStringRecord);
            lastName = lastName.Replace("\0", string.Empty, StringComparison.InvariantCulture);

            char gender = BitConverter.ToChar(reader.ReadBytes(2), 0);
            int year = BitConverter.ToInt32(reader.ReadBytes(4), 0);
            int month = BitConverter.ToInt32(reader.ReadBytes(4), 0);
            int day = BitConverter.ToInt32(reader.ReadBytes(4), 0);
            DateTime dateOfBirth = new DateTime(year, month, day);
            decimal credit = reader.ReadDecimal();
            short duration = reader.ReadInt16();
            var fileCabinetRecord = new FileCabinetRecord(id, firstName, lastName, gender, dateOfBirth, credit, duration);

            return fileCabinetRecord;
        }

        private void FileWriter(FileCabinetRecord fileCabinetRecord, BinaryWriter writer, long position)
        {
            writer.BaseStream.Position = position;
            writer.Write((short)0);
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