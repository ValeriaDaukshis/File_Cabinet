using System.Collections.Generic;
using System.Xml.Serialization;

namespace FileCabinetGenerator.Records
{
    /// <summary>
    /// FileCabinetRecords.
    /// </summary>
    [XmlRoot("records")]
    public class FileCabinetRecords
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecords"/> class.
        /// </summary>
        public FileCabinetRecords()
        {
            this.Record = new List<FileCabinetGenerator.Records.Record>();
        }

        /// <summary>
        /// Gets the record.
        /// </summary>
        /// <value>
        /// The record.
        /// </value>
        [XmlElement("record")]
        public List<FileCabinetGenerator.Records.Record> Record { get; set; }
    }
}