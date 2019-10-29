using System;

namespace FileCabinetApp.CommandHandlers.FunctionalCommandHandlers
{
    public class ExitCommandHandler : CommandHandlerBase
    {
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest.Command == "exit")
            {
                Exit(commandRequest.Parameters);
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(commandRequest);
            }
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            Program.isRunning = false;
        }
    }
}