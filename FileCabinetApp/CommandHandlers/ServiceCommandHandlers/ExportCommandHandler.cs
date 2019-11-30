using System;
using System.IO;

using FileCabinetApp.CommandHandlers.Extensions;
using FileCabinetApp.Service;

namespace FileCabinetApp.CommandHandlers.ServiceCommandHandlers
{
    /// <summary>
    ///     ExportCommandHandler.
    /// </summary>
    /// <seealso cref="FileCabinetApp.CommandHandlers.ServiceCommandHandlerBase" />
    public class ExportCommandHandler : ServiceCommandHandlerBase
    {
        private readonly ModelWriters modelWriter;

        private FileCabinetServiceSnapshot snapshot;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExportCommandHandler" /> class.
        /// </summary>
        /// <param name="cabinetService">The file cabinet service.</param>
        /// <param name="modelWriter">console writer.</param>
        public ExportCommandHandler(IFileCabinetService cabinetService, ModelWriters modelWriter)
            : base(cabinetService)
        {
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
            if (!CommandHandlersExtensions.ImportExportParametersSpliter(parameters, out var fileFormat, out var path, "export"))
            {
                return;
            }

            try
            {
                using (var stream = new StreamWriter(path))
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
                        this.modelWriter.LineWriter.Invoke($"{fileFormat} writer is not found");
                        return;
                    }
                }

                this.modelWriter.LineWriter.Invoke($"File {path} was successfully exported");
            }
            catch (IOException ex)
            {
                this.modelWriter.LineWriter.Invoke(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                this.modelWriter.LineWriter.Invoke(ex.Message);
            }
        }
    }
}