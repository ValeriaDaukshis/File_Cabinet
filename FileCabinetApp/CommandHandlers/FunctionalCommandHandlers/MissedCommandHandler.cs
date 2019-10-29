using System;

namespace FileCabinetApp.CommandHandlers.FunctionalCommandHandlers
{
    public class MissedCommandHandler : CommandHandlerBase
    {
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