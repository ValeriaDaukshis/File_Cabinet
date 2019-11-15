using System;
using System.Globalization;
using System.IO;
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
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const string ServiceStorageFile = "cabinet-records.db";
        private const string ValidationRulesFile = "validation-rules.json";

        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;
        private static bool isRunning = true;
        private static IFileCabinetService fileCabinetService;

        private static Action<bool> running = b => isRunning = b;

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
            var recordPrinter = new DefaultRecordPrinter();
            var helpCommand = new HelpCommandHandler();
            var createCommand = new CreateCommandHandler(fileCabinetService);
            var insertCommand = new InsertCommandHandler(fileCabinetService);
            var getCommand = new GetCommandHandler(fileCabinetService, recordPrinter);
            var editCommand = new EditCommandHandler(fileCabinetService);
            var updateCommand = new UpdateCommandHandler(fileCabinetService);
            var removeCommand = new RemoveCommandHandler(fileCabinetService);
            var deleteCommand = new DeleteCommandHandler(fileCabinetService);
            var findCommand = new FindCommandHandler(fileCabinetService, recordPrinter);
            var statCommand = new StatCommandHandler(fileCabinetService);
            var importCommand = new ImportCommandHandler(fileCabinetService);
            var exportCommand = new ExportCommandHandler(fileCabinetService);
            var purgeCommand = new PurgeCommandHandler(fileCabinetService);
            var exitCommand = new ExitCommandHandler(fileCabinetService as IDisposable, running);
            var missedCommand = new MissedCommandHandler();

            helpCommand.SetNext(createCommand);
            createCommand.SetNext(insertCommand);
            insertCommand.SetNext(getCommand);
            getCommand.SetNext(editCommand);
            editCommand.SetNext(updateCommand);
            updateCommand.SetNext(removeCommand);
            removeCommand.SetNext(deleteCommand);
            deleteCommand.SetNext(findCommand);
            findCommand.SetNext(statCommand);
            statCommand.SetNext(importCommand);
            importCommand.SetNext(exportCommand);
            exportCommand.SetNext(purgeCommand);
            purgeCommand.SetNext(exitCommand);
            exitCommand.SetNext(missedCommand);

            return helpCommand;
        }

        private static string CreateValidPath(string path) =>
            Path.Combine(Directory.GetCurrentDirectory(), path);
    }
}
