using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using FileCabinetApp.Service;
using FileCabinetApp.Validators;

namespace FileCabinetApp
{
    /// <summary>
    /// Enter point.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Valeria Daukshis";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;
        private static readonly CultureInfo Culture = CultureInfo.CurrentCulture;
        private static readonly CultureInfo DateTimeCulture = new CultureInfo("en-US");

        private static bool isRunning = true;
        private static IRecordValidator recordValidator = new DefaultValidator();
        private static IFileCabinetService fileCabinetService;

        /// <summary>
        /// The commands.
        /// </summary>
        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("get", Get),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
            new Tuple<string, Action<string>>("export", ExportToCsv),
        };

        /// <summary>
        /// The help messages.
        /// </summary>
        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "prints the record statistics", "The 'stat' command prints the record statistics." },
            new string[] { "create", "creates a new record", "The 'create' command creates a new record." },
            new string[] { "get", "get all record", "The 'get' command get all record." },
            new string[] { "edit", "edit record by id", "The 'edit' command edit record by id." },
            new string[] { "export", "export data to csv file", "The 'export' command export data to csv file." },
            new string[]
            {
                "find", "find record by firstname/lastname/dateofbirth",
                "The 'find' command find record by firstname/lastname/dateofbirth.",
            },
        };

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            ReadValidationRules(args);
            fileCabinetService = new FileCabinetService();
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

            do
            {
                Console.Write("> ");
                var inputs = Console.ReadLine().Split(' ', 2);
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HintMessage);
                    continue;
                }

                var index = Array.FindIndex(commands, 0, commands.Length, i => i.Item1.Equals(command, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    commands[index].Item2(parameters);
                }
                else
                {
                    PrintMissedCommandInfo(command);
                }
            }
            while (isRunning);
        }

        private static void ExportToCsv(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                Console.WriteLine("No parameters after command 'export'");
                return;
            }

            string[] inputParameters = parameters.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

            if (inputParameters.Length < 2)
            {
                Console.WriteLine("Not enough parameters after command 'find'");
                return;
            }

            string importFile = inputParameters[0].ToLower(Culture);
            string path = inputParameters[1];
            try
            {
                using (StreamWriter stream = new StreamWriter(path))
                {
                    FileCabinetServiceSnapshot snapshot;
                    if (importFile == "csv")
                    {
                        snapshot = fileCabinetService.MakeSnapshot();
                        snapshot.SaveToCsv(stream);
                    }
                    else if (importFile == "xml")
                    {
                        snapshot = fileCabinetService.MakeSnapshot();
                        snapshot.SaveToXml(stream);
                    }
                    else
                    {
                        Console.WriteLine($"{importFile} writer is not found");
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void ReadValidationRules(string[] arguments)
        {
            if (arguments.Length > 0 && arguments.Contains("-v"))
            {
                if (arguments[1].ToLower(Culture) == "custom")
                {
                    recordValidator = new CustomValidator();
                }
            }
            else if (arguments.Length > 0 && arguments.Contains("--validation-rules"))
            {
                string rule = arguments[0].Split('=')[1];
                if (rule.ToLower(Culture) == "custom")
                {
                    recordValidator = new CustomValidator();
                }
            }
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][Program.ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in helpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[Program.CommandHelpIndex], helpMessage[Program.DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            isRunning = false;
        }

        private static void Find(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                Console.WriteLine("No parameters after command 'find'");
                return;
            }

            parameters = parameters.ToLower(Culture);
            string[] inputParameters = parameters.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (inputParameters.Length < 2)
            {
                Console.WriteLine("Not enough parameters after command 'find'");
                return;
            }

            string field = inputParameters[0];
            string value = inputParameters[1];

            if (value[0] == '"')
            {
                value = value.Substring(1, value.Length - 2);
            }

            ReadOnlyCollection<FileCabinetRecord> foundResult = new ReadOnlyCollection<FileCabinetRecord>(Array.Empty<FileCabinetRecord>());

            if (field == "firstname")
            {
                foundResult = Program.fileCabinetService.FindByFirstName(value);
            }
            else if (field == "lastname")
            {
                foundResult = Program.fileCabinetService.FindByLastName(value);
            }
            else if (field == "dateofbirth")
            {
                DateTime.TryParse(value, DateTimeCulture, DateTimeStyles.None, out var date);
                foundResult = Program.fileCabinetService.FindByDateOfBirth(date);
            }

            if (foundResult.Count > 0)
            {
                PrintRecords(foundResult);
            }

            if (foundResult.Count == 0)
            {
                Console.WriteLine($"{value} is not found");
            }
        }

        private static void Edit(string parameters)
        {
            var records = Program.fileCabinetService.GetRecords();
            if (records.Count == 0)
            {
                Console.WriteLine("There is no records.");
                return;
            }

            if (string.IsNullOrEmpty(parameters))
            {
                Console.WriteLine("Write a record number");
                return;
            }

            if (!int.TryParse(parameters.Trim(), out var id))
            {
                Console.WriteLine($"#{parameters} record is not found");
                return;
            }

            foreach (var rec in records)
            {
                if (rec.Id == id)
                {
                    try
                    {
                        PrintInputFields(out string firstName, out string lastName, out char gender, out DateTime dateOfBirth, out decimal credit, out short duration);
                        Program.fileCabinetService.EditRecord(new FileCabinetRecord(firstName, lastName, gender, dateOfBirth, credit, duration));
                        Console.WriteLine($"Record #{parameters} is updated");
                        return;
                    }
                    catch (ArgumentNullException ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine($"Record #{parameters} is not updated ");
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine($"Record #{parameters} is not updated");
                    }
                }
            }
        }

        private static void Stat(string parameters)
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void Get(string parameters)
        {
            var records = Program.fileCabinetService.GetRecords();
            if (records.Count == 0)
            {
                Console.WriteLine("There is no records.");
                return;
            }

            PrintRecords(records);
        }

        private static void PrintRecords(ReadOnlyCollection<FileCabinetRecord> records)
        {
            foreach (var rec in records)
            {
                Console.WriteLine($"Id: {rec.Id}");
                Console.WriteLine($"\tFirst name: {rec.FirstName}");
                Console.WriteLine($"\tLast name: {rec.LastName}");
                Console.WriteLine($"\tGender: {rec.Gender}");
                Console.WriteLine($"\tDate of birth: {rec.DateOfBirth:yyyy-MMMM-dd}");
                Console.WriteLine($"\tCredit sum: {rec.CreditSum}");
                Console.WriteLine($"\tCredit duration: {rec.Duration}");
            }
        }

        private static void Create(string parameters)
        {
            PrintInputFields(out string firstName, out string lastName, out char gender, out DateTime dateOfBirth, out decimal credit, out short duration);
            try
            {
                var recordNumber =
                    Program.fileCabinetService.CreateRecord(new FileCabinetRecord(firstName, lastName, gender, dateOfBirth, credit, duration));
                Console.WriteLine($"Record #{recordNumber} is created.");
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Record is not created ");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Record is not created ");
            }
        }

        private static void PrintInputFields(out string firstName, out string lastName, out char gender, out DateTime dateOfBirth, out decimal credit, out short duration)
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

        private static Func<string, Tuple<bool, string, string>> stringConverter = input =>
        {
            bool success = !string.IsNullOrEmpty(input) && input.Trim().Length != 0;
            return new Tuple<bool, string, string>(success, input, input.Trim());
        };

        private static Func<string, Tuple<bool, string, char>> charConverter = input =>
        {
            var result = default(char);
            bool success = char.TryParse(input?.Trim().ToUpper(Culture), out result);
            return new Tuple<bool, string, char>(success, input, result);
        };

        private static Func<string, Tuple<bool, string, DateTime>> dateTimeConverter = input =>
        {
            var result = default(DateTime);
            bool success = !string.IsNullOrEmpty(input) && DateTime.TryParse(input, DateTimeCulture, DateTimeStyles.None, out result);
            return new Tuple<bool, string, DateTime>(success, input, result);
        };

        private static Func<string, Tuple<bool, string, decimal>> decimalConverter = input =>
        {
            var result = default(decimal);
            bool success = !string.IsNullOrEmpty(input) && decimal.TryParse(input, out result);
            return new Tuple<bool, string, decimal>(success, input, result);
        };

        private static Func<string, Tuple<bool, string, short>> shortConverter = input =>
        {
            var result = default(short);
            bool success = !string.IsNullOrEmpty(input) && short.TryParse(input, out result);
            return new Tuple<bool, string, short>(success, input, result);
        };

        private static Func<string, Tuple<bool, string>> firstNameValidator = input =>
        {
            try
            {
                recordValidator.ValidateFirstName(input);
            }
            catch (ArgumentNullException ex)
            {
                return new Tuple<bool, string>(false, ex.Message);
            }
            catch (ArgumentException ex)
            {
                return new Tuple<bool, string>(false, ex.Message);
            }

            return new Tuple<bool, string>(true, input);
        };

        private static Func<string, Tuple<bool, string>> lastNameValidator = input =>
        {
            try
            {
                recordValidator.ValidateLastName(input);
            }
            catch (ArgumentNullException ex)
            {
                return new Tuple<bool, string>(false, ex.Message);
            }
            catch (ArgumentException ex)
            {
                return new Tuple<bool, string>(false, ex.Message);
            }

            return new Tuple<bool, string>(true, input);
        };

        private static Func<char, Tuple<bool, string>> genderValidator = input =>
        {
            try
            {
                recordValidator.ValidateGender(input);
            }
            catch (ArgumentException ex)
            {
                return new Tuple<bool, string>(false, ex.Message);
            }

            return new Tuple<bool, string>(true, $"{input}");
        };

        private static Func<decimal, Tuple<bool, string>> creditSumValidator = input =>
        {
            try
            {
                recordValidator.ValidateCreditSum(input);
            }
            catch (ArgumentException ex)
            {
                return new Tuple<bool, string>(false, ex.Message);
            }

            return new Tuple<bool, string>(true, $"{input}");
        };

        private static Func<DateTime, Tuple<bool, string>> dateOfBirthValidator = input =>
        {
            try
            {
                recordValidator.ValidateDateOfBirth(input);
            }
            catch (ArgumentException ex)
            {
                return new Tuple<bool, string>(false, ex.Message);
            }

            return new Tuple<bool, string>(true, $"{input:MM/dd/yyyy}");
        };

        private static Func<short, Tuple<bool, string>> durationValidator = input =>
        {
            try
            {
                recordValidator.ValidateDuration(input);
            }
            catch (ArgumentException ex)
            {
                return new Tuple<bool, string>(false, ex.Message);
            }

            return new Tuple<bool, string>(true, $"{input}");
        };

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
