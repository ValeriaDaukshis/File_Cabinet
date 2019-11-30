using System;
using System.Globalization;
using System.IO;
using System.Xml;
using FileCabinetApp.Records;

namespace FileCabinetApp.FIleWriters
{
    /// <summary>
    /// FileCabinetRecordXmlWriter.
    /// </summary>
    public class FileCabinetRecordXmlWriter
    {
        private readonly StreamWriter writer;
        private XmlWriter xmlWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlWriter"/> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public FileCabinetRecordXmlWriter(StreamWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Writes the header.
        /// </summary>
        public void WriteHeader()
        {
            XmlWriterSettings settings = new XmlWriterSettings() { Indent = true };
            this.xmlWriter = XmlWriter.Create(this.writer, settings);
            this.xmlWriter.WriteStartElement("records");
        }

        /// <summary>
        /// Writes the specified record.
        /// </summary>
        /// <param name="record">The record.</param>
        public void Write(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException($"{nameof(record)} is null");
            }

            this.xmlWriter.WriteStartElement("record");
            this.xmlWriter.WriteAttributeString("id",  $"{record.Id}");
            this.xmlWriter.WriteStartElement("name");
            this.xmlWriter.WriteAttributeString("first",  $"{record.FirstName}");
            this.xmlWriter.WriteAttributeString("last",  $"{record.LastName}");
            this.xmlWriter.WriteEndElement();
            this.xmlWriter.WriteElementString("gender", $"{record.Gender}");
            this.xmlWriter.WriteElementString("dateOfBirth", record.DateOfBirth.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            this.xmlWriter.WriteElementString("creditSum", $"{record.CreditSum}");
            this.xmlWriter.WriteElementString("duration", $"{record.Duration}");
            this.xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// Writes the footer.
        /// </summary>
        public void WriteFooter()
        {
            this.xmlWriter.WriteEndElement();
            this.xmlWriter.Dispose();
            this.xmlWriter.Close();
        }
    }
}