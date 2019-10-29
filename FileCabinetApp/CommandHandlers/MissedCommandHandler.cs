using System;
using FileCabinetApp.Service;

namespace FileCabinetApp.CommandHandlers
{
    public class MissedCommandHandler : CommandHandlerBase
    {
        private IFileCabinetService fileCabinetService;

        public MissedCommandHandler(IFileCabinetService fileCabinetService)
        {
            this.fileCabinetService = fileCabinetService;
        }

        public override void Handle(AppCommandRequest commandRequest)
        {
            PrintMissedCommandInfo(commandRequest.Command);
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }
    }
}