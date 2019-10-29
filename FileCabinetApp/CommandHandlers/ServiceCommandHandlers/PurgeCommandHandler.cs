using System;
using FileCabinetApp.Service;

namespace FileCabinetApp.CommandHandlers.ServiceCommandHandlers
{
    public class PurgeCommandHandler : ServiceCommandHandlerBase
    {
        public PurgeCommandHandler(IFileCabinetService fileCabinetService) : base(fileCabinetService)
        {
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