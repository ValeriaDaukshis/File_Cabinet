using System;

namespace FileCabinetApp.CommandHandlers.FunctionalCommandHandlers
{
    /// <summary>
    ///     ExitCommandHandler.
    /// </summary>
    /// <seealso cref="CommandHandlerBase" />
    public class ExitCommandHandler : CommandHandlerBase
    {
        private readonly IDisposable dispose;

        private readonly Action<bool> isRunning;

        private readonly ModelWriters modelWriter;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExitCommandHandler" /> class.
        /// </summary>
        /// <param name="isRunning">The is running.</param>
        /// <param name="dispose">Dispose.</param>
        /// <param name="modelWriter">console writer.</param>
        public ExitCommandHandler(IDisposable dispose, Action<bool> isRunning, ModelWriters modelWriter)
        {
            this.dispose = dispose;
            this.isRunning = isRunning;
            this.modelWriter = modelWriter;
        }

        /// <summary>
        ///     Handles the specified command request.
        /// </summary>
        /// <param name="commandRequest">The command request.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is null)
            {
                throw new ArgumentNullException(nameof(commandRequest), $"{nameof(commandRequest)} is null");
            }

            if (commandRequest.Command == "exit")
            {
                this.Exit(commandRequest.Parameters);
            }
            else
            {
                this.NextHandler.Handle(commandRequest);
            }
        }

        private void Exit(string parameters)
        {
            this.dispose?.Dispose();
            this.modelWriter.LineWriter.Invoke("Exiting an application...");
            this.isRunning.Invoke(false);
        }
    }
}