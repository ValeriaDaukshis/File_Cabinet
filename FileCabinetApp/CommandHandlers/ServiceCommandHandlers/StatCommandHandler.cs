using System;
using FileCabinetApp.Service;

namespace FileCabinetApp.CommandHandlers.ServiceCommandHandlers
{
    public class StatCommandHandler : ServiceCommandHandlerBase
    {
        public StatCommandHandler(IFileCabinetService fileCabinetService) : base(fileCabinetService)
        {
        }

        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest.Command == "stat")
            {
                Stat(commandRequest.Parameters);
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(commandRequest);
            }
        }

        private void Stat(string parameters)
        {
            (int purgedRecords, int recordsCount) = this.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s), where {purgedRecords} are purged.");
        }
    }
}