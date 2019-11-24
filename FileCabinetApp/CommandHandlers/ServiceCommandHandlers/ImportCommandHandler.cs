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
        private string xsdValidatorFile;
        private IXsdValidator xsdValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportCommandHandler"/> class.
        /// </summary>
        /// <param name="cabinetService">The cabinet service.</param>
        /// <param name="xsdValidator">The XSD validator.</param>
        /// <param name="xsdValidatorFile">The XSD validator file.</param>
        public ImportCommandHandler(IFileCabinetService cabinetService, IXsdValidator xsdValidator, string xsdValidatorFile)
            : base(cabinetService)
        {
            this.xsdValidatorFile = xsdValidatorFile;
            this.xsdValidator = xsdValidator;
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
                        Snapshot = this.CabinetService.MakeSnapshot();
                        Snapshot.LoadFromCsv(stream, RecordValidator, this.Converter);
                        int count = this.CabinetService.Restore(Snapshot);
                        Console.WriteLine($"{count} records were imported from {path}");
                    }
                    else if (fileFormat == "xml")
                    {
                        Snapshot = this.CabinetService.MakeSnapshot();
                        this.xsdValidator.ValidateXml(this.xsdValidatorFile, path);
                        Snapshot.LoadFromXml(stream, RecordValidator);
                        int count = this.CabinetService.Restore(Snapshot);
                        Console.WriteLine($"{count} records were imported from {path}");
                    }
                    else
                    {
                        Console.WriteLine($"{fileFormat} writer is not found");
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("File wasn't imported");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("File wasn't imported");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("File wasn't imported");
            }
            catch (XmlException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("File wasn't imported");
            }
        }
    }
}