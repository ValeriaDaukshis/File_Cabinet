using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using FileCabinetApp.ExceptionClasses;
using FileCabinetApp.Records;
using FileCabinetApp.Validators;

namespace FileCabinetApp.Service
{
    /// <summary>
    /// FileCabinetFilesystemService.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Service.IFileCabinetService" />
    public sealed class FileCabinetFilesystemService : IFileCabinetService, IDisposable
    {
        private const int SizeOfStringRecord = 120;
        private const long RecordSize = 278;
        private readonly FileStream fileStream;
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();
        private readonly Dictionary<int, long> recordIndexPosition = new Dictionary<int, long>();
        private readonly IRecordValidator validator;
        private readonly CultureInfo culture = CultureInfo.InvariantCulture;
        private readonly BinaryReader reader;
        private readonly BinaryWriter writer;
        private int countOfRecords;
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="fileStream">The file stream.</param>
        /// <param name="validator">The validator.</param>
        public FileCabinetFilesystemService(FileStream fileStream, IRecordValidator validator)
        {
            this.fileStream = fileStream;
            this.reader = new BinaryReader(this.fileStream);
            this.writer = new BinaryWriter(this.fileStream);
            this.validator = validator;
            this.countOfRecords = 0;
        }

        /// <summary>
        /// Creates the record.
        /// </summary>
        /// <param name="record">The file cabinet record.</param>
        /// <returns>
        /// Id of created record.
        /// </returns>
        public int CreateRecord(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), $"{nameof(record)} is null");
            }

            this.validator.ValidateParameters(record);
            record.Id = this.GenerateId(record);
            this.countOfRecords++;

            long position = RecordSize * (this.countOfRecords - 1);
            this.FileWriter(record, position);
            this.writer.Flush();

            this.recordIndexPosition.Add(record.Id, position);
            this.AddValueToDictionary(record.FirstName, this.firstNameDictionary, record);
            this.AddValueToDictionary(record.LastName, this.lastNameDictionary, record);
            this.AddValueToDictionary(record.DateOfBirth, this.dateOfBirthDictionary, record);

            return record.Id;
        }

        /// <summary>
        /// Removes the record.
        /// </summary>
        /// <param name="record">The record.</param>
        public void RemoveRecord(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), $"{nameof(record)} is null");
            }

            long position = this.recordIndexPosition[record.Id];
            this.writer.BaseStream.Position = position;
            short reserved = 1;
            this.writer.Write(reserved);
            this.writer.Flush();

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

            this.recordIndexPosition.Clear();
            this.writer.BaseStream.Position = 0;
            this.reader.BaseStream.Position = 0;

            long readerPosition = 0;
            long sizeOfFile = 0;
            int countOfDeletedRecords = 0;

            while (readerPosition != this.reader.BaseStream.Length)
            {
                rewriteRecord.Enqueue(readerPosition);
                FileCabinetRecord record = this.FileReader(readerPosition);
                if (record != null)
                {
                    long recordPosition = rewriteRecord.Dequeue();
                    this.FileWriter(record, recordPosition);
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
            if (fileCabinetRecord is null)
            {
                throw new ArgumentNullException(nameof(fileCabinetRecord), $"{nameof(fileCabinetRecord)} is null");
            }

            if (!this.recordIndexPosition.ContainsKey(fileCabinetRecord.Id))
            {
                throw new FileRecordNotFoundException(fileCabinetRecord.Id);
            }

            long position = this.recordIndexPosition[fileCabinetRecord.Id];
            FileCabinetRecord oldRecord = this.FileReader(position);

            this.writer.BaseStream.Position = position + sizeof(short) + sizeof(int);
            if (fileCabinetRecord.FirstName != oldRecord.FirstName)
            {
                var buffer = Encoding.Unicode.GetBytes(CreateEmptyString(fileCabinetRecord.FirstName, 60));
                this.writer.Write(buffer);
                this.RemoveValueFromDictionary(oldRecord.FirstName, this.firstNameDictionary, oldRecord);
                this.AddValueToDictionary(fileCabinetRecord.FirstName, this.firstNameDictionary, fileCabinetRecord);
            }
            else
            {
                this.writer.BaseStream.Position += SizeOfStringRecord;
            }

            if (fileCabinetRecord.LastName != oldRecord.LastName)
            {
                var buffer = Encoding.Unicode.GetBytes(CreateEmptyString(fileCabinetRecord.LastName, 60));
                this.writer.Write(buffer);
                this.RemoveValueFromDictionary(oldRecord.LastName, this.lastNameDictionary, oldRecord);
                this.AddValueToDictionary(fileCabinetRecord.LastName, this.lastNameDictionary, fileCabinetRecord);
            }
            else
            {
                this.writer.BaseStream.Position += SizeOfStringRecord;
            }

            if (fileCabinetRecord.Gender != oldRecord.Gender)
            {
                var buffer = Encoding.Unicode.GetBytes(CreateEmptyString(fileCabinetRecord.Gender.ToString(this.culture), 1));
                this.writer.Write(buffer);
            }
            else
            {
                this.writer.BaseStream.Position += sizeof(char); // 2
            }

            if (fileCabinetRecord.DateOfBirth != oldRecord.DateOfBirth)
            {
                this.writer.Write(fileCabinetRecord.DateOfBirth.Year);
                this.writer.Write(fileCabinetRecord.DateOfBirth.Month);
                this.writer.Write(fileCabinetRecord.DateOfBirth.Day);

                this.RemoveValueFromDictionary(oldRecord.DateOfBirth, this.dateOfBirthDictionary, oldRecord);
                this.AddValueToDictionary(fileCabinetRecord.DateOfBirth, this.dateOfBirthDictionary, fileCabinetRecord);
            }
            else
            {
                this.writer.BaseStream.Position += sizeof(int) * 3; // 12
            }

            if (fileCabinetRecord.CreditSum != oldRecord.CreditSum)
            {
                this.writer.Write(fileCabinetRecord.CreditSum);
            }
            else
            {
                this.writer.BaseStream.Position += sizeof(decimal); // 16
            }

            if (fileCabinetRecord.Duration != oldRecord.Duration)
            {
                this.writer.Write(fileCabinetRecord.Duration);
            }

            this.writer.Flush();
        }

        /// <summary>
        /// GetStat.
        /// </summary>
        /// <returns>reserved records, count of records.</returns>
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

            this.reader.BaseStream.Position = 0;
            long position = this.reader.BaseStream.Position;
            for (int i = 0; i < this.countOfRecords; i++)
            {
                FileCabinetRecord record = this.FileReader(position);
                if (record != null)
                {
                    list.Add(record);
                }

                position += RecordSize;
            }

            return list.AsReadOnly();
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

                yield break;
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

                yield break;
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

                yield break;
            }

            foreach (var fileCabinetRecord in Array.Empty<FileCabinetRecord>())
            {
                yield return fileCabinetRecord;
            }
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
            if (snapshot is null)
            {
                throw new ArgumentNullException(nameof(snapshot), $"{nameof(snapshot)} is null");
            }

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

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private static string CreateEmptyString(string s, int capacity)
        {
            StringBuilder builder = new StringBuilder(capacity);
            builder.Append(s);
            for (int i = s.Length; i < builder.Capacity; i++)
            {
                builder.Append('\x0000');
            }

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
                this.reader.Dispose();
            }

            this.disposed = true;
        }

        private FileCabinetRecord FileReader(long pointer)
        {
            this.reader.BaseStream.Position = pointer;
            short reserved = this.reader.ReadInt16();
            if (reserved == 1)
            {
                return null;
            }

            int id = this.reader.ReadInt32();
            string firstName = Encoding.Unicode.GetString(this.reader.ReadBytes(SizeOfStringRecord), 0, SizeOfStringRecord);
            firstName = firstName.Replace("\0", string.Empty, StringComparison.InvariantCulture);

            string lastName = Encoding.Unicode.GetString(this.reader.ReadBytes(SizeOfStringRecord), 0, SizeOfStringRecord);
            lastName = lastName.Replace("\0", string.Empty, StringComparison.InvariantCulture);

            char gender = BitConverter.ToChar(this.reader.ReadBytes(2), 0);
            int year = BitConverter.ToInt32(this.reader.ReadBytes(4), 0);
            int month = BitConverter.ToInt32(this.reader.ReadBytes(4), 0);
            int day = BitConverter.ToInt32(this.reader.ReadBytes(4), 0);
            DateTime dateOfBirth = new DateTime(year, month, day);
            decimal credit = this.reader.ReadDecimal();
            short duration = this.reader.ReadInt16();
            var fileCabinetRecord = new FileCabinetRecord(id, firstName, lastName, gender, dateOfBirth, credit, duration);

            return fileCabinetRecord;
        }

        private void FileWriter(FileCabinetRecord fileCabinetRecord, long position)
        {
            this.writer.BaseStream.Position = position;
            this.writer.Write((short)0);
            this.writer.Write(fileCabinetRecord.Id);

            var buffer = Encoding.Unicode.GetBytes(CreateEmptyString(fileCabinetRecord.FirstName, 60));
            this.writer.Write(buffer);

            buffer = Encoding.Unicode.GetBytes(CreateEmptyString(fileCabinetRecord.LastName, 60));
            this.writer.Write(buffer);

            buffer = Encoding.Unicode.GetBytes(CreateEmptyString(fileCabinetRecord.Gender.ToString(this.culture), 1));
            this.writer.Write(buffer);

            this.writer.Write(fileCabinetRecord.DateOfBirth.Year);
            this.writer.Write(fileCabinetRecord.DateOfBirth.Month);
            this.writer.Write(fileCabinetRecord.DateOfBirth.Day);
            this.writer.Write(fileCabinetRecord.CreditSum);
            this.writer.Write(fileCabinetRecord.Duration);
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