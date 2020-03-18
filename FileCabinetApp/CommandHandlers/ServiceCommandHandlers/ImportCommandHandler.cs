using System;
using System.IO;
using System.Xml;

using FileCabinetApp.CommandHandlers.Extensions;
using FileCabinetApp.Service;
using FileCabinetApp.Validators.XmlFileValidator;

namespace FileCabinetApp.CommandHandlers.ServiceCommandHandlers
{
    /// <summary>
    ///     ImportCommandHandler.
    /// </summary>
    /// <seealso cref="ServiceCommandHandlerBase" />
    public class ImportCommandHandler : ServiceCommandHandlerBase
    {
        private readonly ModelWriters modelWriter;

        private readonly IXmlValidator xmlValidator;

        private readonly string xsdValidatorFile;

        private FileCabinetServiceSnapshot snapshot;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ImportCommandHandler" /> class.
        /// </summary>
        /// <param name="cabinetService">The cabinet service.</param>
        /// <param name="xmlValidator">The XSD validator.</param>
        /// <param name="xsdValidatorFile">The XSD validator file.</param>
        /// <param name="modelWriter">console writer.</param>
        public ImportCommandHandler(IFileCabinetService cabinetService, IXmlValidator xmlValidator, string xsdValidatorFile, ModelWriters modelWriter)
            : base(cabinetService)
        {
            this.xsdValidatorFile = xsdValidatorFile;
            this.xmlValidator = xmlValidator;
            this.modelWriter = modelWriter;
        }

        /// <summary>
        ///     Handles the specified command request.
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
            if (!CommandHandlersExtensions.ImportExportParametersSpliter(parameters, out var fileFormat, out var path, "import"))
            {
                return;
            }

            try
            {
                using (var stream = new StreamReader(path))
                {
                    if (fileFormat == "csv")
                    {
                        this.snapshot = this.CabinetService.MakeSnapshot();
                        this.snapshot.LoadFromCsv(stream, RecordValidator, Converter, this.modelWriter);
                        var count = this.CabinetService.Restore(this.snapshot);
                        this.modelWriter.LineWriter.Invoke($"{count} records were imported from {path}");
                    }
                    else if (fileFormat == "xml")
                    {
                        this.snapshot = this.CabinetService.MakeSnapshot();
                        this.xmlValidator.ValidateXml(this.xsdValidatorFile, path);
                        this.snapshot.LoadFromXml(stream, RecordValidator, this.modelWriter);
                        var count = this.CabinetService.Restore(this.snapshot);
                        this.modelWriter.LineWriter.Invoke($"{count} records were imported from {path}");
                    }
                    else
                    {
                        this.modelWriter.LineWriter.Invoke($"{fileFormat} writer is not found");
                    }
                }
            }
            catch (IOException ex)
            {
                this.modelWriter.LineWriter.Invoke(ex.Message);
                this.modelWriter.LineWriter.Invoke("File wasn't imported");
            }
            catch (UnauthorizedAccessException ex)
            {
                this.modelWriter.LineWriter.Invoke(ex.Message);
                this.modelWriter.LineWriter.Invoke("File wasn't imported");
            }
            catch (ArgumentException ex)
            {
                this.modelWriter.LineWriter.Invoke(ex.Message);
                this.modelWriter.LineWriter.Invoke("File wasn't imported");
            }
            catch (XmlException ex)
            {
                this.modelWriter.LineWriter.Invoke(ex.Message);
                this.modelWriter.LineWriter.Invoke("File wasn't imported");
            }
        }
    }
}