using System;
using System.Globalization;
using System.IO;
using CommandLine;
using FileCabinetApp.CommandHandlers;
using FileCabinetApp.CommandHandlers.FunctionalCommandHandlers;
using FileCabinetApp.CommandHandlers.Printer;
using FileCabinetApp.CommandHandlers.ServiceCommandHandlers;
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
        private static IFileCabinetService fileCabinetMeter;

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
                        fileCabinetMeter = new ServiceMeter(fileCabinetService);
                    }
                    else
                    {
                        fileCabinetMeter = fileCabinetService;
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
            var createCommand = new CreateCommandHandler(fileCabinetMeter);
            var getCommand = new GetCommandHandler(fileCabinetMeter, recordPrinter);
            var editCommand = new EditCommandHandler(fileCabinetMeter);
            var deleteCommand = new DeleteCommandHandler(fileCabinetMeter);
            var findCommand = new FindCommandHandler(fileCabinetMeter, recordPrinter);
            var statCommand = new StatCommandHandler(fileCabinetMeter);
            var importCommand = new ImportCommandHandler(fileCabinetMeter);
            var exportCommand = new ExportCommandHandler(fileCabinetMeter);
            var purgeCommand = new PurgeCommandHandler(fileCabinetMeter);
            var exitCommand = new ExitCommandHandler(fileCabinetService as IDisposable, running);
            var missedCommand = new MissedCommandHandler();

            helpCommand.SetNext(createCommand);
            createCommand.SetNext(getCommand);
            getCommand.SetNext(editCommand);
            editCommand.SetNext(deleteCommand);
            deleteCommand.SetNext(findCommand);
            findCommand.SetNext(statCommand);
            statCommand.SetNext(importCommand);
            importCommand.SetNext(exportCommand);
            exportCommand.SetNext(purgeCommand);
            purgeCommand.SetNext(exitCommand);
            exitCommand.SetNext(missedCommand);

            return helpCommand;
        }
    }
}
