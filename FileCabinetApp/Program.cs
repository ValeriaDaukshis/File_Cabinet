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

        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;
        private static bool isRunning = true;
        private static IFileCabinetService fileCabinetService;
        private static IExpressionExtensions expressionExtensions;

        private static Action<bool> running = b => isRunning = b;

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen.", "Syntax: help" },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application.", "Syntax: exit" },
            new string[] { "stat", "prints the record statistics", "The 'stat' command prints the record statistics.", "Syntax: stat" },
            new string[] { "create", "creates a new record", "The 'create' command creates a new record.", "Syntax: create" },
            new string[] { "insert", "creates a new record", "The 'insert' command creates a new record.", "Syntax: insert (firstname, lastname, gender, dateofbirth, credit, duration) values ('your values', ...)" },
            new string[] { "get", "get all record", "The 'get' command get all record.", "Syntax: get" },
            new string[] { "edit", "edit record by id", "The 'edit' command edit record by id.", "Syntax: edit (id number)" },
            new string[] { "update", "edit record by field(s)", "The 'update' command edit record by field(s).", "Syntax: update set field = value, field2 = value2 where field3 = value3 (and field4 = value4 ...)" },
            new string[] { "export", "export data to csv/xml file", "The 'export' command export data to csv/xml file.", "Syntax: export csv/xml (nameOfFile)" },
            new string[] { "import", "import data to csv/xml file", "The 'import' command import data from csv/xml file.", "Syntax: import csv/xml (nameOfFile)" },
            new string[] { "remove", "delete record by id", "The 'remove' command delete record by id.", "Syntax: remove (id number)" },
            new string[] { "delete", "delete record by any field", "The 'remove' command delete record by any field.", "Syntax: delete where firstname/lastname/... = value" },
            new string[] { "select", "select records by fields", "The 'select' command select records by fields.", "Syntax: select firstname, lastname, ... where firstname/lastname/... = value" },
            new string[] { "purge", "purge the file", "The 'purge' command purge the file.", "Syntax: purge" },
            new string[] { "syntax", "prints the help syntax screen", "The 'syntax' command prints the help syntax screen.", "Syntax: syntax" },
            new string[]
            {
                "find", "find record by firstname/lastname/dateofbirth",
                "The 'find' command find record by firstname/lastname/dateofbirth.", "Syntax: find firstname/lastname/dateofbirth (value)",
            },
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
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(ValidationRulesFile, optional: true, reloadOnChange: true);
            var config = builder.Build();

            Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed(o =>
                {
                    if (o.ValidationRules != null && o.ValidationRules.ToLower(Culture) == "custom")
                    {
                        (CommandHandlerBase.RecordValidator, ValidatorParams) = new ValidatorBuilder().CreateCustom(config);
                    }
                    else
                    {
                        (CommandHandlerBase.RecordValidator, ValidatorParams) = new ValidatorBuilder().CreateDefault(config);
                    }

                    if (o.Storage != null && o.Storage.ToLower(Culture) == "file")
                    {
                        CommandHandlerBase.ServiceStorageFileStream = new FileStream(ServiceStorageFile, FileMode.OpenOrCreate);
                        fileCabinetService = new FileCabinetFilesystemService(CommandHandlerBase.ServiceStorageFileStream, CommandHandlerBase.RecordValidator);
                    }
                    else
                    {
                        fileCabinetService = new FileCabinetMemoryService(CommandHandlerBase.RecordValidator);
                    }

                    if (o.StopWatcher == true)
                    {
                        fileCabinetService = new ServiceMeter(fileCabinetService);
                    }

                    if (o.Logger == true)
                    {
                        string sourceFilePath = CreateValidPath("logger.log");
                        fileCabinetService = new ServiceLogger(fileCabinetService, sourceFilePath);
                    }
                });

            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

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
            var commands = helpMessages.SelectMany(x => x).Where((c, i) => i % 3 == 0).ToArray();
            var recordPrinter = new DefaultRecordPrinter();
            var tablePrinter = new TablePrinter();
            expressionExtensions = new ExpressionExtensions(fileCabinetService);

            var helpCommand = new HelpCommandHandler(helpMessages);
            var createCommand = new CreateCommandHandler(fileCabinetService);
            var insertCommand = new InsertCommandHandler(fileCabinetService);
            var getCommand = new GetCommandHandler(fileCabinetService, recordPrinter);
            var editCommand = new EditCommandHandler(fileCabinetService);
            var updateCommand = new UpdateCommandHandler(fileCabinetService, expressionExtensions);
            var removeCommand = new RemoveCommandHandler(fileCabinetService);
            var deleteCommand = new DeleteCommandHandler(fileCabinetService, expressionExtensions);
            var findCommand = new FindCommandHandler(fileCabinetService, recordPrinter);
            var selectCommand = new SelectCommandHandler(fileCabinetService, expressionExtensions, tablePrinter);
            var statCommand = new StatCommandHandler(fileCabinetService);
            var importCommand = new ImportCommandHandler(fileCabinetService);
            var exportCommand = new ExportCommandHandler(fileCabinetService);
            var purgeCommand = new PurgeCommandHandler(fileCabinetService);
            var exitCommand = new ExitCommandHandler(fileCabinetService as IDisposable, running);
            var syntaxCommand = new SyntaxCommandHandler(helpMessages);
            var missedCommand = new MissedCommandHandler(commands);

            helpCommand.SetNext(createCommand);
            createCommand.SetNext(insertCommand);
            insertCommand.SetNext(getCommand);
            getCommand.SetNext(editCommand);
            editCommand.SetNext(updateCommand);
            updateCommand.SetNext(removeCommand);
            removeCommand.SetNext(deleteCommand);
            deleteCommand.SetNext(findCommand);
            findCommand.SetNext(selectCommand);
            selectCommand.SetNext(statCommand);
            statCommand.SetNext(importCommand);
            importCommand.SetNext(exportCommand);
            exportCommand.SetNext(purgeCommand);
            purgeCommand.SetNext(syntaxCommand);
            syntaxCommand.SetNext(exitCommand);
            exitCommand.SetNext(missedCommand);

            return helpCommand;
        }

        private static string CreateValidPath(string path) =>
            Path.Combine(Directory.GetCurrentDirectory(), path);
    }
}
