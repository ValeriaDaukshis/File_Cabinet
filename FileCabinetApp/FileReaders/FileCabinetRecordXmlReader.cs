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
        private readonly ModelWriters modelWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlReader"/> class.
        /// </summary>
        /// <param name="streamReader">The reader.</param>
        /// <param name="validator">The validator.</param>
        /// <param name="modelWriter">console writer.</param>
        public FileCabinetRecordXmlReader(StreamReader streamReader, IRecordValidator validator, ModelWriters modelWriter)
        {
            this.streamReader = streamReader;
            this.validator = validator;
            this.modelWriter = modelWriter;
        }

        /// <summary>
        /// Reads all.
        /// </summary>
        /// <returns>list of records.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            XmlSerializer formatter = new XmlSerializer(typeof(RecordsModel));

            RecordsModel recordsModel;
            using (XmlReader xmlReader = XmlReader.Create(this.streamReader))
            {
                recordsModel = (RecordsModel)formatter.Deserialize(xmlReader);
            }

            IList<FileCabinetRecord> list = new List<FileCabinetRecord>();
            foreach (var node in recordsModel.Record)
            {
                try
                {
                    this.ReaderValidator(node);
                    list.Add(new FileCabinetRecord(node.Id, node.Name.FirstName, node.Name.LastName, node.Gender, node.DateOfBirth, node.CreditSum, node.Duration));
                }
                catch (ArgumentException ex)
                {
                    this.modelWriter.LineWriter.Invoke(ex.Message);
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