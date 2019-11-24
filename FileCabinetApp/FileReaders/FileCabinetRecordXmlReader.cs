using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using FileCabinetApp.Records;
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
        private readonly Action<string> consoleWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlReader"/> class.
        /// </summary>
        /// <param name="streamReader">The reader.</param>
        /// <param name="validator">The validator.</param>
        /// <param name="consoleWriter">console writer.</param>
        public FileCabinetRecordXmlReader(StreamReader streamReader, IRecordValidator validator, Action<string> consoleWriter)
        {
            this.streamReader = streamReader;
            this.validator = validator;
            this.consoleWriter = consoleWriter;
        }

        /// <summary>
        /// Reads all.
        /// </summary>
        /// <returns>list of records.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            XmlSerializer formatter = new XmlSerializer(typeof(FileCabinetRecords));

            FileCabinetRecords records;
            using (XmlReader xmlReader = XmlReader.Create(this.streamReader))
            {
                records = (FileCabinetRecords)formatter.Deserialize(xmlReader);
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
                    this.consoleWriter.Invoke(ex.Message);
                }
            }

            return list;
        }

        private void ReaderValidator(Record node)
        {
            this.validator.ValidateParameters(new FileCabinetRecord(node.Id, node.Name.FirstName, node.Name.LastName, node.Gender, node.DateOfBirth, node.CreditSum, node.Duration));
        }
    }
}