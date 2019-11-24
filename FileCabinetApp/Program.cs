using System;
using System.Globalization;
using System.IO;
using System.Linq;
using CommandLine;
using FileCabinetApp.CommandHandlers;
using FileCabinetApp.CommandHandlers.FunctionalCommandHandlers;
using FileCabinetApp.CommandHandlers.Printer;
using FileCabinetApp.CommandHandlers.ServiceCommandHandlers;
using FileCabinetApp.Logger;
using FileCabinetApp.Service;
using FileCabinetApp.Timer;
using FileCabinetApp.Validators;
using FileCabinetApp.Validators.XmlFileValidator;
using Microsoft.Extensions.Configuration;

namespace FileCabinetApp
{
    /// <summary>
    /// Enter point.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Valeria Daukshis";
        private const string HintMessage = "Enter your command, or enter 'help'/'syntax' to get help.";
        private const string ServiceStorageFile = "cabinet-records.db";
        private const string ValidationRulesFile = "validation-rules.json";
        private const string XsdValidatorFile = "records.xsd";

        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;
        private static readonly Action<bool> Running = b => isRunning = b;
        private static readonly ConsoleWriters ConsoleWriters = new ConsoleWriters();

        private static IFileCabinetService fileCabinetService;
        private static IExpressionExtensions expressionExtensions;
        private static IXsdValidator xsdValidator;
        private static ITablePrinter tablePrinter;
        private static string xsdValidatorFile;
        private static bool isRunning = true;

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen.", "Syntax: help" },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application.", "Syntax: exit" },
            new string[] { "stat", "prints the record statistics", "The 'stat' command prints the record statistics.", "Syntax: stat" },
            new string[] { "create", "creates a new record", "The 'create' command creates a new record.", "Syntax: create" },
            new string[] { "insert", "creates a new record", "The 'insert' command creates a new record.", "Syntax: insert (firstname, lastname, gender, dateofbirth, credit, duration) values ('your values', ...)" },
            new string[] { "update", "edit record by field(s)", "The 'update' command edit record by field(s).", "Syntax: update set field = value, field2 = value2 where field3 = value3 (and field4 = value4 ...)" },
            new string[] { "export", "export data to csv/xml file", "The 'export' command export data to csv/xml file.", "Syntax: export csv/xml (nameOfFile)" },
            new string[] { "import", "import data to csv/xml file", "The 'import' command import data from csv/xml file.", "Syntax: import csv/xml (nameOfFile)" },
            new string[] { "delete", "delete record by any field", "The 'remove' command delete record by any field.", "Syntax: delete where firstname/lastname/... = value" },
            new string[] { "select", "select records by fields", "The 'select' command select records by fields.", "Syntax: select firstname, lastname, ... where firstname/lastname/... = value" },
            new string[] { "purge", "purge the file", "The 'purge' command purge the file.", "Syntax: purge" },
            new string[] { "syntax", "prints the help syntax screen", "The 'syntax' command prints the help syntax screen.", "Syntax: syntax" },
        };

        /// <summary>
        /// Gets or sets validatorParams.
        /// </summary>
        /// <value>
        /// ValidatorParams.
        /// </value>
        public static ValidatorParameters ValidatorParams { get; set; }

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>RecordIdValidator
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            xsdValidatorFile = XsdValidatorFile;

            Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed(o =>
                {
                    if (o.ValidationRules != null && o.ValidationRules.ToLower(Culture) == "custom")
                    {
                        (CommandHandlerBase.RecordValidator, ValidatorParams) = new ValidatorBuilder().CreateCustom(ValidationRulesFile);
                        Console.WriteLine("Custom validator");
                    }
                    else
                    {
                        (CommandHandlerBase.RecordValidator, ValidatorParams) = new ValidatorBuilder().CreateDefault(ValidationRulesFile);
                        Console.WriteLine("Default validator");
                    }

                    if (o.Storage != null && o.Storage.ToLower(Culture) == "file")
                    {
                        CommandHandlerBase.ServiceStorageFileStream = new FileStream(ServiceStorageFile, FileMode.OpenOrCreate);
                        fileCabinetService = new FileCabinetFilesystemService(CommandHandlerBase.ServiceStorageFileStream, CommandHandlerBase.RecordValidator);
                        Console.WriteLine("Used filesystem service");
                    }
                    else
                    {
                        fileCabinetService = new FileCabinetMemoryService(CommandHandlerBase.RecordValidator);
                        Console.WriteLine("Used memory service");
                    }

                    if (o.StopWatcher)
                    {
                        fileCabinetService = new ServiceMeter(fileCabinetService, ConsoleWriters);
                        Console.WriteLine("Used timer");
                    }

                    if (o.Logger)
                    {
                        string sourceFilePath = CreateValidPath("logger.log");
                        fileCabinetService = new ServiceLogger(fileCabinetService, sourceFilePath, ConsoleWriters);
                        Console.WriteLine("Used logger");
                    }
                });

            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

            tablePrinter = new DefaultTablePrinter(ConsoleWriters.LineWriter);
            expressionExtensions = new ExpressionExtensions(fileCabinetService);
            xsdValidator = new XmlValidator();

            ICommandHandler commandHandler = CreateCommandHandlers();
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

                const int parametersIndex = 1;
                var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                commandHandler.Handle(new AppCommandRequest
                    {
                        Command = command,
                        Parameters = parameters,
                    });
            }
            while (isRunning);
        }

        private static ICommandHandler CreateCommandHandlers()
        {
            var commands = helpMessages.SelectMany(x => x).Where((c, i) => i % 4 == 0).ToArray();

            var helpCommand = new HelpCommandHandler(helpMessages, ConsoleWriters);

            helpCommand.SetNext(new CreateCommandHandler(fileCabinetService, ConsoleWriters))
                .SetNext(new InsertCommandHandler(fileCabinetService, ConsoleWriters))
                .SetNext(new UpdateCommandHandler(fileCabinetService, expressionExtensions, ConsoleWriters))
                .SetNext(new DeleteCommandHandler(fileCabinetService, expressionExtensions, ConsoleWriters))
                .SetNext(new SelectCommandHandler(fileCabinetService, expressionExtensions, tablePrinter, ConsoleWriters))
                .SetNext(new StatCommandHandler(fileCabinetService, ConsoleWriters))
                .SetNext(new ImportCommandHandler(fileCabinetService, xsdValidator, xsdValidatorFile, ConsoleWriters))
                .SetNext(new ExportCommandHandler(fileCabinetService, ConsoleWriters))
                .SetNext(new PurgeCommandHandler(fileCabinetService, ConsoleWriters))
                .SetNext(new SyntaxCommandHandler(helpMessages, ConsoleWriters))
                .SetNext(new ExitCommandHandler(fileCabinetService as IDisposable, Running, ConsoleWriters))
                .SetNext(new MissedCommandHandler(commands, ConsoleWriters));

            return helpCommand;
        }

        private static string CreateValidPath(string path) =>
            Path.Combine(Directory.GetCurrentDirectory(), path);
    }
}
