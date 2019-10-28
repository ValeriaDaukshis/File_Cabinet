using System;

namespace FileCabinetApp.CommandHandlers
{
    public class PurgeCommandHandler : CommandHandlerBase
    {
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

        private static void Purge(string parameters)
        {
            (int countOfDeletedRecords, int countOfRecords) = Program.fileCabinetService.PurgeDeletedRecords();
            Console.WriteLine($"Data file processing is completed: {countOfDeletedRecords} of {countOfRecords} records were purged");
        }
    }
}