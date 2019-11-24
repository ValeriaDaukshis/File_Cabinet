using System;
using System.IO;
using System.Xml;
using FileCabinetApp.Service;
using FileCabinetApp.Validators.XmlFileValidator;

namespace FileCabinetApp.CommandHandlers.ServiceCommandHandlers
{
    /// <summary>
    /// ImportCommandHandler.
    /// </summary>
    /// <seealso cref="FileCabinetApp.CommandHandlers.ServiceCommandHandlerBase" />
    public class ImportCommandHandler : ServiceCommandHandlerBase
    {
        private readonly string xsdValidatorFile;
        private readonly IXsdValidator xsdValidator;
        private readonly ConsoleWriters consoleWriter;
        private FileCabinetServiceSnapshot snapshot;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportCommandHandler"/> class.
        /// </summary>
        /// <param name="cabinetService">The cabinet service.</param>
        /// <param name="xsdValidator">The XSD validator.</param>
        /// <param name="xsdValidatorFile">The XSD validator file.</param>
        /// <param name="consoleWriter">console writer.</param>
        public ImportCommandHandler(IFileCabinetService cabinetService, IXsdValidator xsdValidator, string xsdValidatorFile, ConsoleWriters consoleWriter)
            : base(cabinetService)
        {
            this.xsdValidatorFile = xsdValidatorFile;
            this.xsdValidator = xsdValidator;
            this.consoleWriter = consoleWriter;
        }

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

            if (commandRequest.Command == "import")
            {
                this.Import(commandRequest.Parameters);
            }
            else
            {
                this.NextHandler.Handle(commandRequest);
            }
        }

        private void Import(string parameters)
        {
            if (!CommandHandlersExpressions.ImportExportParametersSpliter(parameters, out var fileFormat, out var path))
            {
                return;
            }

            try
            {
                using (StreamReader stream = new StreamReader(path))
                {
                    if (fileFormat == "csv")
                    {
                        this.snapshot = this.CabinetService.MakeSnapshot();
                        this.snapshot.LoadFromCsv(stream, RecordValidator, Converter, this.consoleWriter);
                        int count = this.CabinetService.Restore(this.snapshot);
                        this.consoleWriter.LineWriter.Invoke($"{count} records were imported from {path}");
                    }
                    else if (fileFormat == "xml")
                    {
                        this.snapshot = this.CabinetService.MakeSnapshot();
                        this.xsdValidator.ValidateXml(this.xsdValidatorFile, path);
                        this.snapshot.LoadFromXml(stream, RecordValidator, this.consoleWriter);
                        int count = this.CabinetService.Restore(this.snapshot);
                        this.consoleWriter.LineWriter.Invoke($"{count} records were imported from {path}");
                    }
                    else
                    {
                        this.consoleWriter.LineWriter.Invoke($"{fileFormat} writer is not found");
                    }
                }
            }
            catch (IOException ex)
            {
                this.consoleWriter.LineWriter.Invoke(ex.Message);
                this.consoleWriter.LineWriter.Invoke("File wasn't imported");
            }
            catch (UnauthorizedAccessException ex)
            {
                this.consoleWriter.LineWriter.Invoke(ex.Message);
                this.consoleWriter.LineWriter.Invoke("File wasn't imported");
            }
            catch (ArgumentException ex)
            {
                this.consoleWriter.LineWriter.Invoke(ex.Message);
                this.consoleWriter.LineWriter.Invoke("File wasn't imported");
            }
            catch (XmlException ex)
            {
                this.consoleWriter.LineWriter.Invoke(ex.Message);
                this.consoleWriter.LineWriter.Invoke("File wasn't imported");
            }
        }
    }
}