using System;

namespace FileCabinetApp
{
    /// <summary>
    /// FileCabinetRecord.
    /// </summary>
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
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get;  set; }

        /// <summary>
        /// Gets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        public string FirstName { get; private set; }

        /// <summary>
        /// Gets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        public string LastName { get; private set; }

        /// <summary>
        /// Gets the date of birth.
        /// </summary>
        /// <value>
        /// The date of birth.
        /// </value>
        public DateTime DateOfBirth { get; private set; }

        /// <summary>
        /// Gets the gender.
        /// </summary>
        /// <value>
        /// The gender.
        /// </value>
        public char Gender { get; private set; }

        /// <summary>
        /// Gets duration.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        public short Duration { get; private set; }

        /// <summary>
        /// Gets credit sum.
        /// </summary>
        /// <value>
        /// Credit sum.
        /// </value>
        public decimal CreditSum { get; private set; }
    }
}