using System;
using FileCabinetApp.Service;

namespace FileCabinetApp.CommandHandlers
{
    public class PurgeCommandHandler : CommandHandlerBase
    {
        private IFileCabinetService fileCabinetService;

        public PurgeCommandHandler(IFileCabinetService fileCabinetService)
        {
            this.fileCabinetService = fileCabinetService;
        }

        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest.Command == "purge")
            {
                Purge(commandRequest.Parameters);
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(commandRequest);
            }
        }

        private void Purge(string parameters)
        {
            (int countOfDeletedRecords, int countOfRecords) = this.fileCabinetService.PurgeDeletedRecords();
            Console.WriteLine($"Data file processing is completed: {countOfDeletedRecords} of {countOfRecords} records were purged");
        }
    }
}