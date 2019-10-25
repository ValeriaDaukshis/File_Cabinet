using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using FileCabinetApp.Validators;

namespace FileCabinetApp.FileReaders
{
    /// <summary>
    /// FileCabinetRecordXmlReader.
    /// </summary>
    public class FileCabinetRecordXmlReader
    {
        private readonly StreamReader streamReader;
        private readonly IRecordValidator validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlReader"/> class.
        /// </summary>
        /// <param name="streamReader">The reader.</param>
        /// <param name="validator">The validator.</param>
        public FileCabinetRecordXmlReader(StreamReader streamReader, IRecordValidator validator)
        {
            this.streamReader = streamReader;
            this.validator = validator;
        }

        /// <summary>
        /// Reads all.
        /// </summary>
        /// <returns>list of records.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            XmlSerializer formatter = new XmlSerializer(typeof(FileCabinetRecords));

            FileCabinetRecords records;
            using (this.streamReader)
            {
                records = (FileCabinetRecords)formatter.Deserialize(this.streamReader);
            }

            IList<FileCabinetRecord> list = new List<FileCabinetRecord>();
            foreach (var node in records.Record)
            {
                try
                {
                    this.ReaderValidator(node);
                    list.Add(new FileCabinetRecord(node.Id, node.Name.FirstName, node.Name.LastName, node.Gender, node.DateOfBirth, node.CreditSum, node.Duration));

                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return list;
        }

        private void ReaderValidator(Record record)
        {
            this.validator.ValidateFirstName(record.Name.FirstName);
            this.validator.ValidateLastName(record.Name.LastName);
            this.validator.ValidateCreditSum(record.CreditSum);
            this.validator.ValidateDuration(record.Duration);
            this.validator.ValidateGender(record.Gender);
            this.validator.ValidateDateOfBirth(record.DateOfBirth);
        }
    }
}