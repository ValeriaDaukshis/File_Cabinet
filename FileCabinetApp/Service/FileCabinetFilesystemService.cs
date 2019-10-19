using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace FileCabinetApp.Service
{
    /// <summary>
    /// FileCabinetFilesystemService.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Service.IFileCabinetService" />
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private readonly FileStream fileStream;
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
            fileCabinetRecord.Id = this.countOfRecords + 1;
            this.countOfRecords++;
            BinaryWriter writer = new BinaryWriter(this.fileStream);
            writer.Write(default(short));
            writer.Write(fileCabinetRecord.Id);

            var buffer = Encoding.Unicode.GetBytes(this.CreateEmptyString(fileCabinetRecord.FirstName));
            writer.Write(buffer);

            buffer = Encoding.Unicode.GetBytes(this.CreateEmptyString(fileCabinetRecord.LastName));
            writer.Write(buffer);

            writer.Write(fileCabinetRecord.Gender);
            writer.Write(fileCabinetRecord.DateOfBirth.Year);
            writer.Write(fileCabinetRecord.DateOfBirth.Month);
            writer.Write(fileCabinetRecord.DateOfBirth.Day);
            writer.Write(fileCabinetRecord.CreditSum);
            writer.Write(fileCabinetRecord.Duration);
            writer.Flush();

            return fileCabinetRecord.Id;
        }

        /// <summary>
        /// Edits the record.
        /// </summary>
        /// <param name="fileCabinetRecord">The file cabinet record.</param>
        public void EditRecord(FileCabinetRecord fileCabinetRecord)
        {
            throw new NotImplementedException();
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
            for (int i = 0; i < this.countOfRecords; i++)
            {
                short reserved = reader.ReadInt16();
                int id = reader.ReadInt32();
                string firstName = Encoding.Unicode.GetString(reader.ReadBytes(120), 0, 120);
                string lastName = Encoding.Unicode.GetString(reader.ReadBytes(120), 0, 120);
                char gender = reader.ReadChar();
                int year = BitConverter.ToInt32(reader.ReadBytes(4), 0);
                int month = BitConverter.ToInt32(reader.ReadBytes(4), 0);
                int day = BitConverter.ToInt32(reader.ReadBytes(4), 0);
                DateTime dateOfBirth = new DateTime(year, month, day);
                decimal credit = reader.ReadDecimal();
                short duration = reader.ReadInt16();
                var fileCabinetRecord = new FileCabinetRecord(firstName, lastName, gender, dateOfBirth, credit, duration);
                fileCabinetRecord.Id = id;
                list.Add(fileCabinetRecord);
            }

            reader.Close();

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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Makes the snapshot.
        /// </summary>
        /// <returns>
        /// FileCabinetServiceSnapshot.
        /// </returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            throw new NotImplementedException();
        }

        private string CreateEmptyString(string s)
        {
            StringBuilder builder = new StringBuilder(60);
            builder.Append(s);
            for (int i = s.Length; i < builder.Capacity; i++)
            {
                builder.Append(default(char));
            }

            return builder.ToString();
        }
    }
}