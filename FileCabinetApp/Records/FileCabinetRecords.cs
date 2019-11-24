﻿using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace FileCabinetApp.Records
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
            this.Record = new List<Record>();
        }

        /// <summary>
        /// Gets the record.
        /// </summary>
        /// <value>
        /// The record.
        /// </value>
        [XmlElement("record")]
        public List<Record> Record { get; }
    }
}