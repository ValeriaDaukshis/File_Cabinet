﻿using System;
using System.Globalization;

namespace FileCabinetApp
{
    public static class Program
    {
        private const string DeveloperName = "Valeria Daukshis";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static bool isRunning = true;
        private static FileCabinetService fileCabinetService = new FileCabinetService();

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("get", Get),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "prints the record statistics", "The 'stat' command prints the record statistics." },
            new string[] { "create", "creates a new record", "The 'create' command creates a new record." },
            new string[] { "get", "get all record", "The 'get' command get all record." },
        };

        public static void Main(string[] args)
        {
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

        private static void Stat(string parameters)
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void Get(string parameters)
        {
            var records = Program.fileCabinetService.GetRecords();
            if (records.Length == 0)
            {
                Console.WriteLine("There is no records.");
            }
            else
            {
                for (int i = 0; i < records.Length; i++)
                {
                    // Console.WriteLine($"#{records[i].Id}, {records[i].FirstName}, {records[i].LastName}, {records[i].DateOfBirth:yyyy-MMMM-dd}");fg
                    Console.WriteLine($"Id: {records[i].Id}");
                    Console.WriteLine($"\tFirst name: {records[i].FirstName}");
                    Console.WriteLine($"\tLast name: {records[i].LastName}");
                    Console.WriteLine($"\tGender: {records[i].Gender}");
                    Console.WriteLine($"\tDate of birth: {records[i].DateOfBirth:yyyy-MMMM-dd}");
                    Console.WriteLine($"\tCredit sum: {records[i].CreditSum}");
                    Console.WriteLine($"\tCredit duration: {records[i].Duration}");
                }
            }
        }

        private static void Create(string parameters)
        {
            Console.Write("First name: ");
            string firstName = CheckStringInput();
            Console.Write("Last name: ");
            string lastName = CheckStringInput();
            Console.Write("Gender(M/F): ");
            char gender = CheckCharInput();
            Console.Write("Date of birth(mm/dd/yyyy): ");
            DateTime dateOfBirth = CheckDateInput();
            Console.Write("Credit sum(bel rub): ");
            decimal credit = CheckDecimalInput();
            Console.Write("Credit duration(month): ");
            short duration = CheckShortInput();

            var recordNumber = Program.fileCabinetService.CreateRecord(firstName, lastName, gender, dateOfBirth, credit, duration);
            Console.WriteLine($"Record #{recordNumber} is created.");
        }

        private static string CheckStringInput()
        {
            bool success;
            string parameters;
            do
            {
                parameters = Console.ReadLine();
                success = !string.IsNullOrEmpty(parameters) ? true : false;
            }
            while (!success);

            return parameters;
        }

        private static short CheckShortInput()
        {
            bool success;
            short value = 0;
            do
            {
                var parameters = Console.ReadLine();
                success = !string.IsNullOrEmpty(parameters) ? true : false;
                if (success)
                {
                    if (!short.TryParse(parameters, out value))
                    {
                        success = false;
                    }
                }
            }
            while (!success);

            return value;
        }

        private static decimal CheckDecimalInput()
        {
            bool success;
            decimal sum = 0;
            do
            {
                var parameters = Console.ReadLine();
                success = !string.IsNullOrEmpty(parameters) ? true : false;
                if (success)
                {
                    if (!decimal.TryParse(parameters, out sum))
                    {
                        success = false;
                    }
                }
            }
            while (!success);

            return sum;
        }

        private static char CheckCharInput()
        {
            bool success;
            char parameters;
            do
            {
                success = char.TryParse(Console.ReadLine()?.ToUpper(CultureInfo.CurrentCulture), out parameters) ? true : false;
                if (success)
                {
                    if (parameters != 'M' && parameters != 'F' && parameters != 'm' && parameters != 'f')
                    {
                        success = false;
                    }
                }
            }
            while (!success);

            return parameters;
        }

        private static DateTime CheckDateInput()
        {
            bool success;
            string parameters;
            DateTime date = default(DateTime);
            do
            {
                parameters = Console.ReadLine();
                success = !string.IsNullOrEmpty(parameters) ? true : false;
                if (success)
                {
                    success = DateTime.TryParse(parameters, new CultureInfo("en-US"), DateTimeStyles.None, out date);
                }
            }
            while (!success);

            return date;
        }
    }
}
