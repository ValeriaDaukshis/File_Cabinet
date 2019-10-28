using System;

namespace FileCabinetApp.CommandHandlers
{
    public class GetCommandHandler : CommandHandlerBase
    {
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest.Command == "get")
            {
                Get(commandRequest.Parameters);
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(commandRequest);
            }
        }

        private static void Get(string parameters)
        {
            var records = Program.fileCabinetService.GetRecords();
            if (records.Count == 0)
            {
                Console.WriteLine("There is no records.");
                return;
            }

            PrintRecords(records);
        }
    }
}