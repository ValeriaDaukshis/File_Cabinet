using System.Collections.Generic;
using System.Xml.Serialization;

namespace FileCabinetApp.Records
{
    /// <summary>
    ///     FileCabinetRecords.
    /// </summary>
    [XmlRoot("records")]
    public class RecordsModel
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RecordsModel" /> class.
        /// </summary>
        public RecordsModel()
        {
            this.Record = new List<Record>();
        }

        /// <summary>
        ///     Gets the record.
        /// </summary>
        /// <value>
        ///     The record.
        /// </value>
        [XmlElement("record")]
        public List<Record> Record { get; }
    }
}