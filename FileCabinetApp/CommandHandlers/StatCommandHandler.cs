using System;

namespace FileCabinetApp.CommandHandlers
{
    public class StatCommandHandler : CommandHandlerBase
    {
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

        private static void Stat(string parameters)
        {
            (int purgedRecords, int recordsCount) = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s), where {purgedRecords} are purged.");
        }
    }
}