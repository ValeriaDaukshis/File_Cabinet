﻿using System;
using System.Xml.Serialization;

namespace FileCabinetApp.Records
{
    /// <summary>
    /// FileCabinetRecord.
    /// </summary>
    [XmlRoot("addressBook")]
    public class FileCabinetRecord
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecord"/> class.
        /// </summary>
        /// <param name="firstName">first name.</param>
        /// <param name="lastName">last name.</param>
        /// <param name="gender">gender.</param>
        /// <param name="dateOfBirth">date of birth.</param>
        /// <param name="credit">credit sum.</param>
        /// <param name="duration">duration.</param>
        public FileCabinetRecord(string firstName, string lastName, char gender, DateTime dateOfBirth, decimal credit, short duration)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.DateOfBirth = dateOfBirth;
            this.Gender = gender;
            this.CreditSum = credit;
            this.Duration = duration;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecord"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="gender">The gender.</param>
        /// <param name="dateOfBirth">The date of birth.</param>
        /// <param name="credit">The credit.</param>
        /// <param name="duration">The duration.</param>
        public FileCabinetRecord(int id, string firstName, string lastName, char gender, DateTime dateOfBirth, decimal credit, short duration)
        : this(firstName, lastName, gender, dateOfBirth, credit, duration)
        {
            this.Id = id;
        }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [XmlElement("records")]
        public int Id { get;  set; }

        /// <summary>
        /// Gets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        [XmlAttribute("first")]
        public string FirstName { get; private set; }

        /// <summary>
        /// Gets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        [XmlAttribute("last")]
        public string LastName { get; private set; }

        /// <summary>
        /// Gets the gender.
        /// </summary>
        /// <value>
        /// The gender.
        /// </value>
        [XmlElement("gender")]
        public char Gender { get; private set; }

        /// <summary>
        /// Gets the date of birth.
        /// </summary>
        /// <value>
        /// The date of birth.
        /// </value>
        [XmlElement("dateOfBirth")]
        public DateTime DateOfBirth { get; private set; }

        /// <summary>
        /// Gets credit sum.
        /// </summary>
        /// <value>
        /// Credit sum.
        /// </value>
        [XmlElement("creditSum")]
        public decimal CreditSum { get; private set; }

        /// <summary>
        /// Gets duration.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        [XmlElement("duration")]
        public short Duration { get; private set; }
    }
}