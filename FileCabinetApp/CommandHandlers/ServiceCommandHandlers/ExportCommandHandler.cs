using System;
using System.IO;
using FileCabinetApp.Service;

namespace FileCabinetApp.CommandHandlers.ServiceCommandHandlers
{
    /// <summary>
    /// ExportCommandHandler.
    /// </summary>
    /// <seealso cref="FileCabinetApp.CommandHandlers.ServiceCommandHandlerBase" />
    public class ExportCommandHandler : ServiceCommandHandlerBase
    {
        private readonly ConsoleWriters consoleWriter;
        private FileCabinetServiceSnapshot snapshot;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportCommandHandler"/> class.
        /// </summary>
        /// <param name="cabinetService">The file cabinet service.</param>
        /// <param name="consoleWriter">console writer.</param>
        public ExportCommandHandler(IFileCabinetService cabinetService, ConsoleWriters consoleWriter)
            : base(cabinetService)
        {
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

            if (commandRequest.Command == "export")
            {
                this.Export(commandRequest.Parameters);
            }
            else
            {
                this.NextHandler.Handle(commandRequest);
            }
        }

        private void Export(string parameters)
        {
            if (!CommandHandlersExpressions.ImportExportParametersSpliter(parameters, out var fileFormat, out var path))
            {
                return;
            }

            try
            {
                using (StreamWriter stream = new StreamWriter(path))
                {
                    if (fileFormat == "csv")
                    {
                        this.snapshot = this.CabinetService.MakeSnapshot();
                        this.snapshot.SaveToCsv(stream);
                    }
                    else if (fileFormat == "xml")
                    {
                        this.snapshot = this.CabinetService.MakeSnapshot();
                        this.snapshot.SaveToXml(stream);
                    }
                    else
                    {
                        this.consoleWriter.LineWriter.Invoke($"{fileFormat} writer is not found");
                        return;
                    }
                }

                this.consoleWriter.LineWriter.Invoke($"File {path} was successfully exported");
            }
            catch (IOException ex)
            {
                this.consoleWriter.LineWriter.Invoke(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                this.consoleWriter.LineWriter.Invoke(ex.Message);
            }
        }
    }
}