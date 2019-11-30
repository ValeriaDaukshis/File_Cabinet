using System;
using System.Globalization;
using System.IO;
using System.Linq;

using CommandLine;

using FileCabinetApp.CommandHandlers;
using FileCabinetApp.CommandHandlers.Extensions;
using FileCabinetApp.CommandHandlers.FunctionalCommandHandlers;
using FileCabinetApp.CommandHandlers.ServiceCommandHandlers;
using FileCabinetApp.FileCabinetServiceWrappers.Logger;
using FileCabinetApp.FileCabinetServiceWrappers.Timer;
using FileCabinetApp.Printer;
using FileCabinetApp.Service;
using FileCabinetApp.Validators;
using FileCabinetApp.Validators.XmlFileValidator;

namespace FileCabinetApp
{
    /// <summary>
    ///     Enter point.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Valeria Daukshis";

        private const string HintMessage = "Enter your command, or enter 'help'/'syntax' to get help.";

        private const string ServiceStorageFile = "cabinet-records.db";

        private const string ValidationRulesFile = "validation-rules.json";

        private const string XsdValidatorFile = "records.xsd";

        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

        private static readonly string[][] HelpMessages =
            {
                new[] { "help", "prints the help screen", "The 'help' command prints the help screen.", "Syntax: help" },
                new[] { "exit", "exits the application", "The 'exit' command exits the application.", "Syntax: exit" },
                new[] { "stat", "prints the record statistics", "The 'stat' command prints the record statistics.", "Syntax: stat" },
                new[] { "create", "creates a new record", "The 'create' command creates a new record.", "Syntax: create" },
                new[] { "insert", "creates a new record", "The 'insert' command creates a new record.", "Syntax: insert (firstname, lastname, gender, dateofbirth, credit, duration) values ('your values', ...)" },
                new[] { "update", "edit record by field(s)", "The 'update' command edit record by field(s).", "Syntax: update set field = value, field2 = value2 where field3 = value3 (and field4 = value4 ...)" },
                new[] { "export", "export data to csv/xml file", "The 'export' command export data to csv/xml file.", "Syntax: export csv/xml (nameOfFile)" },
                new[] { "import", "import data to csv/xml file", "The 'import' command import data from csv/xml file.", "Syntax: import csv/xml (nameOfFile)" },
                new[] { "delete", "delete record by any field", "The 'remove' command delete record by any field.", "Syntax: delete where firstname/lastname/... = value" },
                new[] { "select", "select records by fields", "The 'select' command select records by fields.", "Syntax: select firstname, lastname, ... where firstname/lastname/... = value" },
                new[] { "purge", "purge the file", "The 'purge' command purge the file.", "Syntax: purge" },
                new[] { "syntax", "prints the help syntax screen", "The 'syntax' command prints the help syntax screen.", "Syntax: syntax" },
            };

        private static readonly Action<bool> Running = b => isRunning = b;

        private static IExpressionExtensions expressionExtensions;

        private static IFileCabinetService fileCabinetService;

        private static bool isRunning = true;

        private static ModelWriters modelWriters;

        private static ITablePrinter tablePrinter;

        private static IXmlValidator xmlValidator;

        private static string xsdValidatorFile;

        /// <summary>
        ///     Gets or sets validatorParams.
        /// </summary>
        /// <value>
        ///     ValidatorParams.
        /// </value>
        public static ValidatorParameters ValidatorParams { get; set; }

        /// <summary>
        ///     Defines the entry point of the application.
        /// </summary>
        /// RecordIdValidator
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            xsdValidatorFile = XsdValidatorFile;
            InitModelWriters();

            Parser.Default.ParseArguments<CommandLineOptions>(args)
                  .WithParsed(
                      o =>
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
                                  fileCabinetService = new ServiceMeter(fileCabinetService, modelWriters);
                                  Console.WriteLine("Used timer");
                              }

                              if (o.Logger)
                              {
                                  var sourceFilePath = CreateValidPath("logger.log");
                                  fileCabinetService = new ServiceLogger(fileCabinetService, sourceFilePath);
                                  Console.WriteLine("Used logger");
                              }
                          });

            Console.WriteLine($"File Cabinet Application, developed by {DeveloperName}");
            Console.WriteLine(HintMessage);
            Console.WriteLine();

            tablePrinter = new DefaultTablePrinter(modelWriters.LineWriter);
            expressionExtensions = new ExpressionExtensions(fileCabinetService);
            xmlValidator = new XmlValidator();

            var commandHandler = CreateCommandHandlers();
            do
            {
                Console.Write("> ");
                var inputs = Console.ReadLine()?.Split(' ', 2);
                const int CommandIndex = 0;
                if (inputs != null)
                {
                    var command = inputs[CommandIndex];

                    if (string.IsNullOrEmpty(command))
                    {
                        Console.WriteLine(HintMessage);
                        continue;
                    }

                    const int ParametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[ParametersIndex] : string.Empty;
                    commandHandler.Handle(new AppCommandRequest { Command = command, Parameters = parameters });
                }
            }
            while (isRunning);
        }

        private static ICommandHandler CreateCommandHandlers()
        {
            var commands = HelpMessages.SelectMany(x => x).Where((c, i) => i % 4 == 0).ToArray();

            var helpCommand = new HelpCommandHandler(HelpMessages, modelWriters);

            helpCommand.SetNext(new CreateCommandHandler(fileCabinetService, modelWriters))
                       .SetNext(new InsertCommandHandler(fileCabinetService, modelWriters))
                       .SetNext(new UpdateCommandHandler(fileCabinetService, expressionExtensions, modelWriters))
                       .SetNext(new DeleteCommandHandler(fileCabinetService, expressionExtensions, modelWriters))
                       .SetNext(new SelectCommandHandler(fileCabinetService, expressionExtensions, tablePrinter, modelWriters))
                       .SetNext(new StatCommandHandler(fileCabinetService, modelWriters))
                       .SetNext(new ImportCommandHandler(fileCabinetService, xmlValidator, xsdValidatorFile, modelWriters))
                       .SetNext(new ExportCommandHandler(fileCabinetService, modelWriters))
                       .SetNext(new PurgeCommandHandler(fileCabinetService, modelWriters))
                       .SetNext(new SyntaxCommandHandler(HelpMessages, modelWriters))
                       .SetNext(new ExitCommandHandler(fileCabinetService as IDisposable, Running, modelWriters))
                       .SetNext(new MissedCommandHandler(commands, modelWriters));

            return helpCommand;
        }

        private static string CreateValidPath(string path)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), path);
        }

        private static void InitModelWriters()
        {
            modelWriters = new ModelWriters { Reader = Console.ReadLine, Writer = Console.Write, LineWriter = Console.WriteLine };
        }
    }
}