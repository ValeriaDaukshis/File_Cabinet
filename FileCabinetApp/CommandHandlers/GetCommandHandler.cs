using System;
using FileCabinetApp.Service;

namespace FileCabinetApp.CommandHandlers
{
    public class GetCommandHandler : CommandHandlerBase
    {
        private IFileCabinetService fileCabinetService;

        public GetCommandHandler(IFileCabinetService fileCabinetService)
        {
            this.fileCabinetService = fileCabinetService;
        }

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

        private void Get(string parameters)
        {
            var records = this.fileCabinetService.GetRecords();
            if (records.Count == 0)
            {
                Console.WriteLine("There is no records.");
                return;
            }

            PrintRecords(records);
        }
    }
}