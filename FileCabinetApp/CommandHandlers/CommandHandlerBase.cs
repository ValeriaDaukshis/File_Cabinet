using System;
using System.Globalization;
using System.IO;
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
        /// DateTimeCulture.
        /// </summary>
        protected static readonly CultureInfo DateTimeCulture = new CultureInfo("en-US");

        private static Func<string, Tuple<bool, string, string>> stringConverter = input =>
        {
            bool success = !string.IsNullOrEmpty(input) && input.Trim().Length != 0;
            return new Tuple<bool, string, string>(success, input, input.Trim());
        };

        private static Func<string, Tuple<bool, string, char>> charConverter = input =>
        {
            var result = '\x0000';
            bool success = char.TryParse(input?.Trim().ToUpper(Culture), out result);
            return new Tuple<bool, string, char>(success, input, result);
        };

        private static Func<string, Tuple<bool, string, DateTime>> dateTimeConverter = input =>
        {
            var result = DateTime.Now;
            bool success = !string.IsNullOrEmpty(input) && DateTime.TryParse(input, DateTimeCulture, DateTimeStyles.None, out result);
            return new Tuple<bool, string, DateTime>(success, input, result);
        };

        private static Func<string, Tuple<bool, string, decimal>> decimalConverter = input =>
        {
            var result = 0.0m;
            bool success = !string.IsNullOrEmpty(input) && decimal.TryParse(input, out result);
            return new Tuple<bool, string, decimal>(success, input, result);
        };

        private static Func<string, Tuple<bool, string, short>> shortConverter = input =>
        {
            short result = 0;
            bool success = !string.IsNullOrEmpty(input) && short.TryParse(input, out result);
            return new Tuple<bool, string, short>(success, input, result);
        };

        private static Func<string, Tuple<bool, string>> firstNameValidator = input =>
        {
            if (string.IsNullOrEmpty(input))
            {
                return new Tuple<bool, string>(false, "First name is empty");
            }

            if (input.Length < Program.ValidatorParams.MinLength || input.Length > Program.ValidatorParams.MaxLength)
            {
                return new Tuple<bool, string>(false, $"First name length is upper than {Program.ValidatorParams.MaxLength} or under than {Program.ValidatorParams.MinLength} symbols");
            }

            return new Tuple<bool, string>(true, input);
        };

        private static Func<string, Tuple<bool, string>> lastNameValidator = input =>
        {
            if (string.IsNullOrEmpty(input))
            {
                return new Tuple<bool, string>(false, "Last name is empty");
            }

            if (input.Length < Program.ValidatorParams.MinLength || input.Length > Program.ValidatorParams.MaxLength)
            {
                return new Tuple<bool, string>(false, $"Last name length is upper than {Program.ValidatorParams.MaxLength} or under than {Program.ValidatorParams.MinLength} symbols");
            }

            return new Tuple<bool, string>(true, input);
        };

        private static Func<char, Tuple<bool, string>> genderValidator = gender =>
        {
            if (gender != 'M' && gender != 'F')
            {
                return new Tuple<bool, string>(false, "Indefinite gender");
            }

            return new Tuple<bool, string>(true, $"{gender}");
        };

        private static Func<decimal, Tuple<bool, string>> creditSumValidator = input =>
        {
            if (input > Program.ValidatorParams.MaxCreditSum || input < Program.ValidatorParams.MinCreditSum)
            {
                return new Tuple<bool, string>(false, $"Credit sum is upper than {Program.ValidatorParams.MaxCreditSum} or under than {Program.ValidatorParams.MinCreditSum} BYN");
            }

            return new Tuple<bool, string>(true, $"{input}");
        };

        private static Func<DateTime, Tuple<bool, string>> dateOfBirthValidator = input =>
        {
            if (input > Program.ValidatorParams.MaxDateOfBirth)
            {
                return new Tuple<bool, string>(false, $"Date of birth is upper than {Program.ValidatorParams.MaxDateOfBirth}");
            }

            if (input < Program.ValidatorParams.MinDateOfBirth)
            {
                return new Tuple<bool, string>(false, $"Date of birth is under than {Program.ValidatorParams.MinDateOfBirth}");
            }

            return new Tuple<bool, string>(true, $"{input:MM/dd/yyyy}");
        };

        private static Func<short, Tuple<bool, string>> durationValidator = input =>
        {
            if (input > Program.ValidatorParams.MaxPeriod || input < Program.ValidatorParams.MinPeriod)
            {
                return new Tuple<bool, string>(false, $"Duration is upper than {Program.ValidatorParams.MaxPeriod} or under than {Program.ValidatorParams.MinPeriod}");
            }

            return new Tuple<bool, string>(true, $"{input}");
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
        /// Sets the next.
        /// </summary>
        /// <param name="commandHandler">The command handler.</param>
        public void SetNext(ICommandHandler commandHandler)
        {
            this.NextHandler = commandHandler;
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
        protected static void PrintInputFields(out string firstName, out string lastName, out char gender, out DateTime dateOfBirth, out decimal credit, out short duration)
        {
            Console.Write("First name: ");
            firstName = ReadInput(stringConverter, firstNameValidator);
            Console.Write("Last name: ");
            lastName = ReadInput(stringConverter, lastNameValidator);
            Console.Write("Gender(M/F): ");
            gender = ReadInput(charConverter, genderValidator);
            Console.Write("Date of birth(mm/dd/yyyy): ");
            dateOfBirth = ReadInput(dateTimeConverter, dateOfBirthValidator);
            Console.Write("Credit sum(bel rub): ");
            credit = ReadInput(decimalConverter, creditSumValidator);
            Console.Write("Credit duration(month): ");
            duration = ReadInput(shortConverter, durationValidator);
        }

        /// <summary>
        /// Imports the export parameters spliter.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="fileFormat">The file format.</param>
        /// <param name="path">The path.</param>
        /// <returns>true if params are valid.</returns>
        protected static bool ImportExportParametersSpliter(string parameters, out string fileFormat, out string path)
        {
            fileFormat = string.Empty;
            path = string.Empty;
            if (string.IsNullOrEmpty(parameters))
            {
                Console.WriteLine("No parameters after command 'export'");
                return false;
            }

            string[] inputParameters = parameters.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

            if (inputParameters.Length < 2)
            {
                Console.WriteLine("Not enough parameters after command 'find'");
                return false;
            }

            fileFormat = inputParameters[0].ToLower(Culture);
            path = inputParameters[1];
            return true;
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