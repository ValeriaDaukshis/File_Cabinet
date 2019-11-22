using System;
using System.Globalization;
using System.Xml.Serialization;
using FileCabinetApp.Records;

namespace FileCabinetGenerator.Records
{
    /// <summary>
    /// Record.
    /// </summary>
    public class Record
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Record"/> class.
        /// </summary>
        /// <param name="firstName">first name.</param>
        /// <param name="lastName">last name.</param>
        /// <param name="gender">gender.</param>
        /// <param name="dateOfBirth">date of birth.</param>
        /// <param name="credit">credit sum.</param>
        /// <param name="duration">duration.</param>
        public Record(string firstName, string lastName, char gender, DateTime dateOfBirth, decimal credit, short duration)
        {
            this.Name = new Names { FirstName = firstName, LastName = lastName };
            this.DateOfBirth = dateOfBirth.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            this.Gender = gender;
            this.CreditSum = credit;
            this.Duration = duration;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Record"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="gender">The gender.</param>
        /// <param name="dateOfBirth">The date of birth.</param>
        /// <param name="credit">The credit.</param>
        /// <param name="duration">The duration.</param>
        public Record(int id, string firstName, string lastName, char gender, DateTime dateOfBirth, decimal credit, short duration)
            : this(firstName, lastName, gender, dateOfBirth, credit, duration)
        {
            this.Id = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Record"/> class.
        /// </summary>
        public Record()
        {
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [XmlAttribute("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [XmlElement("name")]
        public Names Name { get; set; }

        /// <summary>
        /// Gets the gender.
        /// </summary>
        /// <value>
        /// The gender.
        /// </value>
        [XmlElement(typeof(char), ElementName = "gender")]
        public char Gender { get; set; }

        /// <summary>
        /// Gets or sets Date of birth of <see cref="FileCabinetRecord"/>.
        /// </summary>
        /// <value>Date of birth.</value>
        [XmlElement(ElementName = "dateOfBirth")]
        public string DateOfBirth { get; set; }

        /// <summary>
        /// Gets credit sum.
        /// </summary>
        /// <value>
        /// Credit sum.
        /// </value>
        [XmlElement("creditSum")]
        public decimal CreditSum { get; set; }

        /// <summary>
        /// Gets duration.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        [XmlElement("duration")]
        public short Duration { get; set; }
    }
}