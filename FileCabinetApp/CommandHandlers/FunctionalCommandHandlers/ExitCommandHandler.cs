using System;

namespace FileCabinetApp.CommandHandlers.FunctionalCommandHandlers
{
    public class ExitCommandHandler : CommandHandlerBase
    {
        private readonly Action<bool> isRunning;

        public ExitCommandHandler(Action<bool> isRunning)
        {
            this.isRunning = isRunning;
        }

        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest.Command == "exit")
            {
                this.Exit(commandRequest.Parameters);
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(commandRequest);
            }
        }

        private void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            this.isRunning.Invoke(false);
        }
    }
}