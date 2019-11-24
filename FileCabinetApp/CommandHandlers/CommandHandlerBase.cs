using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using FileCabinetApp.Converters;
using FileCabinetApp.Memoization;
using FileCabinetApp.Records;
using FileCabinetApp.Service;
using FileCabinetApp.Validators;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// CommandHandlerBase.
    /// </summary>
    /// <seealso cref="FileCabinetApp.CommandHandlers.ICommandHandler" />
    public abstract class CommandHandlerBase : ICommandHandler
    {
        /// <summary>
        /// culture info.
        /// </summary>
        protected static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

        /// <summary>
        /// The fields case dictionary.
        /// </summary>
        protected static readonly Dictionary<string, string> FieldsCaseDictionary = new Dictionary<string, string>
        {
            { "id", "Id" },
            { "firstname", "FirstName" },
            { "lastname", "LastName" },
            { "gender", "Gender" },
            { "dateofbirth", "DateOfBirth" },
            { "creditsum", "CreditSum" },
            { "duration", "Duration" },
        };

        private static readonly Func<string, Tuple<bool, string>> FirstNameValidator = input =>
        {
            if (string.IsNullOrEmpty(input))
            {
                return new Tuple<bool, string>(false, "First name is empty");
            }

            if (input.Length < Program.ValidatorParams.FirstName.Min || input.Length > Program.ValidatorParams.FirstName.Max)
            {
                return new Tuple<bool, string>(false, $"First name length is upper than {Program.ValidatorParams.FirstName.Max} or under than {Program.ValidatorParams.FirstName.Min} symbols");
            }

            return new Tuple<bool, string>(true, input);
        };

        private static readonly Func<string, Tuple<bool, string>> LastNameValidator = input =>
        {
            if (string.IsNullOrEmpty(input))
            {
                return new Tuple<bool, string>(false, "Last name is empty");
            }

            if (input.Length < Program.ValidatorParams.LastName.Min || input.Length > Program.ValidatorParams.LastName.Max)
            {
                return new Tuple<bool, string>(false, $"Last name length is upper than {Program.ValidatorParams.LastName.Max} or under than {Program.ValidatorParams.LastName.Min} symbols");
            }

            return new Tuple<bool, string>(true, input);
        };

        private static readonly Func<char, Tuple<bool, string>> GenderValidator = gender =>
        {
            if (gender != 'M' && gender != 'F')
            {
                return new Tuple<bool, string>(false, "Indefinite gender");
            }

            return new Tuple<bool, string>(true, $"{gender}");
        };

        private static readonly Func<decimal, Tuple<bool, string>> CreditSumValidator = input =>
        {
            if (input > Program.ValidatorParams.CreditSum.Max || input < Program.ValidatorParams.CreditSum.Min)
            {
                return new Tuple<bool, string>(false, $"Credit sum is upper than {Program.ValidatorParams.CreditSum.Max} or under than {Program.ValidatorParams.CreditSum.Min} BYN");
            }

            return new Tuple<bool, string>(true, $"{input}");
        };

        private static readonly Func<DateTime, Tuple<bool, string>> DateOfBirthValidator = input =>
        {
            if (input > Program.ValidatorParams.DateOfBirth.To)
            {
                return new Tuple<bool, string>(false, $"Date of birth is upper than {Program.ValidatorParams.DateOfBirth.To:MM/dd/yyyy}");
            }

            if (input < Program.ValidatorParams.DateOfBirth.From)
            {
                return new Tuple<bool, string>(false, $"Date of birth is under than {Program.ValidatorParams.DateOfBirth.From:MM/dd/yyyy}");
            }

            return new Tuple<bool, string>(true, $"{input:MM/dd/yyyy}");
        };

        private static readonly Func<short, Tuple<bool, string>> DurationValidator = input =>
        {
            if (input > Program.ValidatorParams.Duration.To || input < Program.ValidatorParams.Duration.From)
            {
                return new Tuple<bool, string>(false, $"Duration is upper than {Program.ValidatorParams.Duration.To} or under than {Program.ValidatorParams.Duration.From}");
            }

            return new Tuple<bool, string>(true, $"{input}");
        };

        private static readonly Func<string[], string[], Tuple<bool, string>> FirstNameSearcher = (fields, values) =>
        {
            if (!fields.Contains("FirstName"))
            {
                return new Tuple<bool, string>(false, "Can not find FirstName member");
            }

            var index = fields.ToList().FindIndex(x => x == "FirstName");
            var item = values[index];

            return new Tuple<bool, string>(true, $"{item}");
        };

        private static readonly Func<string[], string[], Tuple<bool, string>> LastNameSearcher = (fields, values) =>
        {
            if (!fields.Contains("LastName"))
            {
                return new Tuple<bool, string>(false, "Can not find LastName member");
            }

            var index = fields.ToList().FindIndex(x => x == "LastName");
            var item = values[index];

            return new Tuple<bool, string>(true, $"{item}");
        };

        private static readonly Func<string[], string[], Tuple<bool, string>> GenderSearcher = (fields, values) =>
        {
            if (!fields.Contains("Gender"))
            {
                return new Tuple<bool, string>(false, "Can not find Gender member");
            }

            var index = fields.ToList().FindIndex(x => x == "Gender");
            var item = values[index];

            return new Tuple<bool, string>(true, $"{item}");
        };

        private static readonly Func<string[], string[], Tuple<bool, string>> CreditSumSearcher = (fields, values) =>
        {
            if (!fields.Contains("CreditSum"))
            {
                return new Tuple<bool, string>(false, "Can not find CreditSum member");
            }

            var index = fields.ToList().FindIndex(x => x == "CreditSum");
            var item = values[index];

            return new Tuple<bool, string>(true, $"{item}");
        };

        private static readonly Func<string[], string[], Tuple<bool, string>> DateOfBirthSearcher = (fields, values) =>
        {
            if (!fields.Contains("DateOfBirth"))
            {
                return new Tuple<bool, string>(false, "Can not find DateOfBirth member");
            }

            var index = fields.ToList().FindIndex(x => x == "DateOfBirth");
            var item = values[index];

            return new Tuple<bool, string>(true, $"{item}");
        };

        private static readonly Func<string[], string[], Tuple<bool, string>> DurationSearcher = (fields, values) =>
        {
            if (!fields.Contains("Duration"))
            {
                return new Tuple<bool, string>(false, "Can not find Duration member");
            }

            var index = fields.ToList().FindIndex(x => x == "Duration");
            var item = values[index];

            return new Tuple<bool, string>(true, $"{item}");
        };

        /// <summary>
        /// Gets or sets the record validator.
        /// </summary>
        /// <value>
        /// The record validator.
        /// </value>
        public static IRecordValidator RecordValidator { get;  set; }

        /// <summary>
        /// Gets or sets the service storage file stream.
        /// </summary>
        /// <value>
        /// The service storage file stream.
        /// </value>
        public static FileStream ServiceStorageFileStream { get;  set; }

        /// <summary>
        /// Gets cache.
        /// </summary>
        /// <value>
        /// Cache.
        /// </value>
        protected static DataCaching Cache { get; } = new DataCaching();

        /// <summary>
        /// Gets or sets fileCabinetServiceSnapshot.
        /// </summary>
        /// <value>
        /// FileCabinetServiceSnapshot.
        /// </value>
        protected static FileCabinetServiceSnapshot Snapshot { get; set; }

        /// <summary>
        /// Gets or sets iCommandHandler.
        /// </summary>
        /// <value>
        /// ICommandHandler.
        /// </value>
        protected ICommandHandler NextHandler { get; set; }

        /// <summary>
        /// Gets the converter.
        /// </summary>
        /// <value>
        /// The converter.
        /// </value>
        protected Converter Converter { get; } = new Converter();

        /// <summary>
        /// Gets the command handlers expressions.
        /// </summary>
        /// <value>
        /// The command handlers expressions.
        /// </value>
        protected CommandHandlersExpressions CommandHandlersExpressions { get; } = new CommandHandlersExpressions(FieldsCaseDictionary);

        /// <summary>
        /// Sets the next.
        /// </summary>
        /// <param name="commandHandler">The command handler.</param>
        /// <returns>ICommandHandler.</returns>
        public ICommandHandler SetNext(ICommandHandler commandHandler)
        {
            this.NextHandler = commandHandler;
            return this.NextHandler;
        }

        /// <summary>
        /// Handles the specified command request.
        /// </summary>
        /// <param name="commandRequest">The command request.</param>
        public abstract void Handle(AppCommandRequest commandRequest);

        /// <summary>
        /// PrintInputFields.
        /// </summary>
        /// <param name="firstName">first name.</param>
        /// <param name="lastName">last name.</param>
        /// <param name="gender">gender.</param>
        /// <param name="dateOfBirth">date of birth.</param>
        /// <param name="credit">credit sum.</param>
        /// <param name="duration">duration.</param>
        protected void PrintInputFields(out string firstName, out string lastName, out char gender, out DateTime dateOfBirth, out decimal credit, out short duration)
        {
            Console.Write("First name: ");
            firstName = ReadInput(this.Converter.StringConverter, FirstNameValidator);
            Console.Write("Last name: ");
            lastName = ReadInput(this.Converter.StringConverter, LastNameValidator);
            Console.Write("Gender(M/F): ");
            gender = ReadInput(this.Converter.CharConverter, GenderValidator);
            Console.Write("Date of birth(mm/dd/yyyy): ");
            dateOfBirth = ReadInput(this.Converter.DateTimeConverter, DateOfBirthValidator);
            Console.Write("Credit sum(bel rub): ");
            credit = ReadInput(this.Converter.DecimalConverter, CreditSumValidator);
            Console.Write("Credit duration(month): ");
            duration = ReadInput(this.Converter.ShortConverter, DurationValidator);
        }

        /// <summary>
        /// Prints the input fields.
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <param name="values">The values.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="gender">The gender.</param>
        /// <param name="dateOfBirth">The date of birth.</param>
        /// <param name="credit">The credit.</param>
        /// <param name="duration">The duration.</param>
        protected void PrintInputFields(string[] fields, string[] values, out string firstName, out string lastName, out char gender, out DateTime dateOfBirth, out decimal credit, out short duration)
        {
            firstName = ReadInput(fields, values, FirstNameSearcher, this.Converter.StringConverter, FirstNameValidator);
            lastName = ReadInput(fields, values, LastNameSearcher, this.Converter.StringConverter, LastNameValidator);
            gender = ReadInput(fields, values, GenderSearcher, this.Converter.CharConverter, GenderValidator);
            dateOfBirth = ReadInput(fields, values, DateOfBirthSearcher, this.Converter.DateTimeConverter, DateOfBirthValidator);
            credit = ReadInput(fields, values, CreditSumSearcher, this.Converter.DecimalConverter, CreditSumValidator);
            duration = ReadInput(fields, values, DurationSearcher, this.Converter.ShortConverter, DurationValidator);
        }

        /// <summary>
        /// Checks the input fields.
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <param name="values">The values.</param>
        /// <param name="record">The record.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="gender">The gender.</param>
        /// <param name="dateOfBirth">The date of birth.</param>
        /// <param name="credit">The credit.</param>
        /// <param name="duration">The duration.</param>
        protected void CheckInputFields(string[] fields, string[] values, FileCabinetRecord record, out string firstName, out string lastName, out char gender, out DateTime dateOfBirth, out decimal credit, out short duration)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), $"{nameof(record)} is null");
            }

            firstName = ReadInput(fields, values, record.FirstName, FirstNameSearcher, this.Converter.StringConverter, FirstNameValidator);
            lastName = ReadInput(fields, values, record.LastName, LastNameSearcher, this.Converter.StringConverter, LastNameValidator);
            gender = ReadInput(fields, values, record.Gender, GenderSearcher, this.Converter.CharConverter, GenderValidator);
            dateOfBirth = ReadInput(fields, values, record.DateOfBirth, DateOfBirthSearcher, this.Converter.DateTimeConverter, DateOfBirthValidator);
            credit = ReadInput(fields, values, record.CreditSum, CreditSumSearcher, this.Converter.DecimalConverter, CreditSumValidator);
            duration = ReadInput(fields, values, record.Duration, DurationSearcher, this.Converter.ShortConverter, DurationValidator);
        }

        private static T ReadInput<T>(string[] fields, string[] values, Func<string[], string[], Tuple<bool, string>> finder, Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            T value;
            var inputResult = finder(fields, values);
            if (!inputResult.Item1)
            {
                throw new ArgumentException($"Conversion failed: {inputResult.Item2}. Please, correct your input.");
            }

            string input = inputResult.Item2;

            var conversionResult = converter(input);

            if (!conversionResult.Item1)
            {
                throw new ArgumentException($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
            }

            value = conversionResult.Item3;

            var validationResult = validator(value);
            if (!validationResult.Item1)
            {
                throw new ArgumentException($"Validation failed: {validationResult.Item2}. Please, correct your input.");
            }

            return value;
        }

        private static T ReadInput<T>(string[] fields, string[] values, T record, Func<string[], string[], Tuple<bool, string>> finder, Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            T value;
            var inputResult = finder(fields, values);
            if (!inputResult.Item1)
            {
                return record;
            }

            string input = inputResult.Item2;

            var conversionResult = converter(input);

            if (!conversionResult.Item1)
            {
                throw new ArgumentException($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
            }

            value = conversionResult.Item3;

            var validationResult = validator(value);
            if (!validationResult.Item1)
            {
                throw new ArgumentException($"Validation failed: {validationResult.Item2}. Please, correct your input.");
            }

            return value;
        }

        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                T value;

                var input = Console.ReadLine();
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }
    }
}