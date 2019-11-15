﻿using System;

namespace FileCabinetApp.CommandHandlers.FunctionalCommandHandlers
{
    /// <summary>
    /// HelpCommandHandler.
    /// </summary>
    /// <seealso cref="FileCabinetApp.CommandHandlers.CommandHandlerBase" />
    public class HelpCommandHandler : CommandHandlerBase
    {
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "prints the record statistics", "The 'stat' command prints the record statistics." },
            new string[] { "create", "creates a new record", "The 'create' command creates a new record." },
            new string[] { "insert", "creates a new record", "The 'insert' command creates a new record. Syntax: insert (firstname, lastname, gender, dateofbirth, credit, duration) values ('your values', ...)" },
            new string[] { "get", "get all record", "The 'get' command get all record." },
            new string[] { "edit", "edit record by id", "The 'edit' command edit record by id." },
            new string[] { "export", "export data to csv/xml file", "The 'export' command export data to csv/xml file." },
            new string[] { "import", "import data to csv/xml file", "The 'import' command import data from csv/xml file." },
            new string[] { "remove", "delete record by id", "The 'remove' command delete record by id." },
            new string[] { "purge", "purge the file", "The 'purge' command purge the file." },
            new string[]
            {
                "find", "find record by firstname/lastname/dateofbirth",
                "The 'find' command find record by firstname/lastname/dateofbirth.",
            },
        };

        /// <summary>
        /// Handles the specified command request.
        /// </summary>
        /// <param name="commandRequest">The command request.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is null)
            {
                throw new ArgumentNullException(nameof(commandRequest), $"{nameof(commandRequest)} is null");
            }

            if (commandRequest.Command == "help")
            {
                PrintHelp(commandRequest.Parameters);
            }
            else
            {
                this.NextHandler.Handle(commandRequest);
            }
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][ExplanationHelpIndex]);
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
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[CommandHelpIndex], helpMessage[DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }
    }
}