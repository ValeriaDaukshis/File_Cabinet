﻿using System.Xml.Serialization;

namespace FileCabinetApp.Records
{
    /// <summary>
    /// Names.
    /// </summary>
    public class Name
    {
        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        [XmlAttribute("first")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        [XmlAttribute("last")]
        public string LastName { get; set; }
    }
}