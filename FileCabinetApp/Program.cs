using System;
using System.Globalization;
using System.IO;
using CommandLine;
using FileCabinetApp.CommandHandlers;
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
        public static bool isRunning = true;
        public static IFileCabinetService fileCabinetService;

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            ICommandHandler commandHandler = CreateCommandHandlers();
            Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed(o =>
                {
                    if (o.ValidationRules != null && o.ValidationRules.ToLower(Culture) == "custom")
                    {
                        CommandHandler.RecordValidator = new CustomValidator();
                    }
                    else
                    {
                        CommandHandler.RecordValidator = new DefaultValidator();
                    }

                    if (o.Storage != null && o.Storage.ToLower(Culture) == "file")
                    {
                        CommandHandler.ServiceStorageFileStream = new FileStream(ServiceStorageFile, FileMode.OpenOrCreate);
                        fileCabinetService = new FileCabinetFilesystemService(CommandHandler.ServiceStorageFileStream);
                        CommandHandler.RecordIdValidator = new RecordIdFilesystemValidator(CommandHandler.ServiceStorageFileStream);
                    }
                    else
                    {
                        fileCabinetService = new FileCabinetMemoryService();
                        CommandHandler.RecordIdValidator = new RecordIdMemoryValidator(fileCabinetService);
                    }
                });
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
            var commendHandler = new CommandHandler();
            return commendHandler;
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
