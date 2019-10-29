using System;
using System.IO;
using FileCabinetApp.Service;

namespace FileCabinetApp.CommandHandlers
{
    public class ImportCommandHandler : CommandHandlerBase
    {
        private IFileCabinetService fileCabinetService;

        public ImportCommandHandler(IFileCabinetService fileCabinetService)
        {
            this.fileCabinetService = fileCabinetService;
        }

        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest.Command == "import")
            {
                Import(commandRequest.Parameters);
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(commandRequest);
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
                        snapshot = this.fileCabinetService.MakeSnapshot();
                        snapshot.LoadFromCsv(stream, RecordValidator);
                        int count = this.fileCabinetService.Restore(snapshot);
                        Console.WriteLine($"{count} records were imported from {path}");
                    }
                    else if (fileFormat == "xml")
                    {
                        snapshot = this.fileCabinetService.MakeSnapshot();
                        snapshot.LoadFromXml(stream, RecordValidator);
                        int count = this.fileCabinetService.Restore(snapshot);
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