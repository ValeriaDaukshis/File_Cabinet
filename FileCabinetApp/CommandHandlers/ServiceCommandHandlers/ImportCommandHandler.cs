using System;
using System.IO;
using System.Xml;
using FileCabinetApp.Service;

namespace FileCabinetApp.CommandHandlers.ServiceCommandHandlers
{
    /// <summary>
    /// ImportCommandHandler.
    /// </summary>
    /// <seealso cref="FileCabinetApp.CommandHandlers.ServiceCommandHandlerBase" />
    public class ImportCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">The file cabinet service.</param>
        public ImportCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
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
            if (!ImportExportParametersSpliter(parameters, out var fileFormat, out var path))
            {
                return;
            }

            try
            {
                using (StreamReader stream = new StreamReader(path))
                {
                    if (fileFormat == "csv")
                    {
                        Snapshot = this.FileCabinetService.MakeSnapshot();
                        Snapshot.LoadFromCsv(stream, RecordValidator);
                        int count = this.FileCabinetService.Restore(Snapshot);
                        Console.WriteLine($"{count} records were imported from {path}");
                    }
                    else if (fileFormat == "xml")
                    {
                        Snapshot = this.FileCabinetService.MakeSnapshot();
                        Snapshot.LoadFromXml(stream, RecordValidator);
                        int count = this.FileCabinetService.Restore(Snapshot);
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
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}