using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
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
        private static IRecordValidator validator = new DefaultValidator();
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
            fileCabinetService = new FileCabinetService(validator);
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

        private static void ReadValidationRules(string[] arguments)
        {
            if (arguments.Length > 0 && arguments.Contains("-v"))
            {
                if (arguments[1].ToLower(Culture) == "custom")
                {
                    validator = new CustomValidator();
                }
            }
            else if (arguments.Length > 0 && arguments.Contains("--validation-rules"))
            {
                string rule = arguments[0].Split('=')[1];
                if (rule.ToLower(Culture) == "custom")
                {
                    validator = new CustomValidator();
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
            string[] inputParameters;
            try
            {
                if (string.IsNullOrEmpty(parameters))
                {
                    throw new ArgumentException("No parameters after command 'find'");
                }

                parameters = parameters.Trim().ToLower(Culture);
                inputParameters = parameters.Split(' ', ' ');

                if (inputParameters.Length < 2)
                {
                    throw new ArgumentException("Not enough parameters after command 'find'");
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            string field = inputParameters[0];
            string value = string.Empty;
            for (int i = 1; i < inputParameters.Length; i++)
            {
                if (!string.IsNullOrEmpty(inputParameters[i]))
                {
                    value = inputParameters[i];
                    break;
                }
            }

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
                        Program.fileCabinetService.EditRecord(new FileCabinetRecord
                            { Id = id, FirstName = firstName, LastName = lastName, Gender = gender, DateOfBirth = dateOfBirth, Duration = duration, CreditSum = credit });
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
                    Program.fileCabinetService.CreateRecord(new FileCabinetRecord
                        { FirstName = firstName, LastName = lastName, Gender = gender, DateOfBirth = dateOfBirth, Duration = duration, CreditSum = credit });
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
            firstName = CheckStringValue();
            Console.Write("Last name: ");
            lastName = CheckStringValue();
            Console.Write("Gender(M/F): ");
            gender = CheckCharValue();
            Console.Write("Date of birth(mm/dd/yyyy): ");
            dateOfBirth = CheckDateTimeValue();
            Console.Write("Credit sum(bel rub): ");
            credit = CheckDecimalValue();
            Console.Write("Credit duration(month): ");
            duration = CheckShortValue();
        }

        private static string CheckStringValue()
        {
            bool success;
            string parameters;
            do
            {
                parameters = Console.ReadLine();
                success = !string.IsNullOrEmpty(parameters);
                if (success)
                {
                    success = parameters.Trim().Length != 0;
                }
            }
            while (!success);

            return parameters;
        }

        private static short CheckShortValue()
        {
            bool success;
            short value = 0;
            do
            {
                var parameters = Console.ReadLine();
                success = !string.IsNullOrEmpty(parameters);
                if (success)
                {
                    success = short.TryParse(parameters, out value);
                }
            }
            while (!success);

            return value;
        }

        private static decimal CheckDecimalValue()
        {
            bool success;
            decimal sum = 0;
            do
            {
                var parameters = Console.ReadLine();
                success = !string.IsNullOrEmpty(parameters);
                if (success)
                {
                    success = decimal.TryParse(parameters, out sum);
                }
            }
            while (!success);

            return sum;
        }

        private static char CheckCharValue()
        {
            bool success;
            char parameters;
            do
            {
                string input = Console.ReadLine()?.Trim();
                success = char.TryParse(input?.ToUpper(Culture), out parameters);
            }
            while (!success);

            return parameters;
        }

        private static DateTime CheckDateTimeValue()
        {
            bool success;
            DateTime date = default(DateTime);
            do
            {
                var parameters = Console.ReadLine();
                success = !string.IsNullOrEmpty(parameters);
                if (success)
                {
                    success = DateTime.TryParse(parameters, DateTimeCulture, DateTimeStyles.None, out date);
                }
            }
            while (!success);

            return date;
        }
    }
}
