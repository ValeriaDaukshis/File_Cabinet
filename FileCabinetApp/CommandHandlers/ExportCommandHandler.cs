using System;
using System.IO;

namespace FileCabinetApp.CommandHandlers
{
    public class ExportCommandHandler : CommandHandlerBase
    {
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest.Command == "export")
            {
                Export(commandRequest.Parameters);
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(commandRequest);
            }
        }

        private static void Export(string parameters)
        {
            if (!ImportExportParametersSpliter(parameters, out var fileFormat, out var path))
            {
                return;
            }

            try
            {
                using (StreamWriter stream = new StreamWriter(path))
                {
                    if (fileFormat == "csv")
                    {
                        snapshot = Program.fileCabinetService.MakeSnapshot();
                        snapshot.SaveToCsv(stream);
                    }
                    else if (fileFormat == "xml")
                    {
                        snapshot = Program.fileCabinetService.MakeSnapshot();
                        snapshot.SaveToXml(stream);
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