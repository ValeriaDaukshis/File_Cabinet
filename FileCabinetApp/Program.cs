using System;
using System.Globalization;
using System.IO;
using CommandLine;
using FileCabinetApp.CommandHandlers;
using FileCabinetApp.CommandHandlers.FunctionalCommandHandlers;
using FileCabinetApp.CommandHandlers.Printer;
using FileCabinetApp.CommandHandlers.ServiceCommandHandlers;
using FileCabinetApp.Service;
using FileCabinetApp.Validators;

namespace FileCabinetApp
{
    /// <summary>
    /// Enter point.
    /// Enter point.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Valeria Daukshis";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const string ServiceStorageFile = @"C:\Users\dauks\File-Cabinet\cabinet-records.db";

        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;
        private static bool isRunning = true;
        private static IFileCabinetService fileCabinetService;

        private static Action<bool> running = b => isRunning = b;

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed(o =>
                {
                    if (o.ValidationRules != null && o.ValidationRules.ToLower(Culture) == "custom")
                    {
                        CommandHandlerBase.RecordValidator = new CustomValidator();
                    }
                    else
                    {
                        CommandHandlerBase.RecordValidator = new DefaultValidator();
                    }

                    if (o.Storage != null && o.Storage.ToLower(Culture) == "file")
                    {
                        CommandHandlerBase.ServiceStorageFileStream = new FileStream(ServiceStorageFile, FileMode.OpenOrCreate);
                        fileCabinetService = new FileCabinetFilesystemService(CommandHandlerBase.ServiceStorageFileStream);
                        CommandHandlerBase.RecordIdValidator = new RecordIdFilesystemValidator(CommandHandlerBase.ServiceStorageFileStream);
                    }
                    else
                    {
                        fileCabinetService = new FileCabinetMemoryService();
                        CommandHandlerBase.RecordIdValidator = new RecordIdMemoryValidator(fileCabinetService);
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
            var getCommand = new GetCommandHandler(fileCabinetService, recordPrinter);
            var editCommand = new EditCommandHandler(fileCabinetService);
            var deleteCommand = new DeleteCommandHandler(fileCabinetService);
            var findCommand = new FindCommandHandler(fileCabinetService, recordPrinter);
            var statCommand = new StatCommandHandler(fileCabinetService);
            var importCommand = new ImportCommandHandler(fileCabinetService);
            var exportCommand = new ExportCommandHandler(fileCabinetService);
            var purgeCommand = new PurgeCommandHandler(fileCabinetService);
            var exitCommand = new ExitCommandHandler(running);
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

    /// <summary>
    /// CommandLineOptions.
    /// </summary>
    internal class CommandLineOptions
    {
        /// <summary>
        /// Gets or sets the validation rules.
        /// </summary>
        /// <value>
        /// The validation rules.
        /// </value>
        [Option('v', "validation-rules", Required = false, HelpText = "set validation rules(default/custom)")]
        public string ValidationRules { get; set; }

        /// <summary>
        /// Gets or sets the storage.
        /// </summary>
        /// <value>
        /// The storage.
        /// </value>
        [Option('s', "storage", Required = false, HelpText = "Set storage place (memory/file)")]
        public string Storage { get; set; }
    }
}
