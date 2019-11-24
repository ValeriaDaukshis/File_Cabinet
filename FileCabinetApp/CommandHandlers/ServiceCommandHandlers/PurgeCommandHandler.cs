using System;
using FileCabinetApp.Service;

namespace FileCabinetApp.CommandHandlers.ServiceCommandHandlers
{
    /// <summary>
    /// PurgeCommandHandler.
    /// </summary>
    /// <seealso cref="FileCabinetApp.CommandHandlers.ServiceCommandHandlerBase" />
    public class PurgeCommandHandler : ServiceCommandHandlerBase
    {
        private readonly Action<string> consoleWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="PurgeCommandHandler"/> class.
        /// </summary>
        /// <param name="cabinetService">The file cabinet service.</param>
        /// <param name="consoleWriter">console writer.</param>
        public PurgeCommandHandler(IFileCabinetService cabinetService, Action<string> consoleWriter)
            : base(cabinetService)
        {
            this.consoleWriter = consoleWriter;
        }

        /// <summary>
        /// Handles the specified command request.
        /// </summary>
        /// <param name="commandRequest">The command request.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is null)
            {
                throw new ArgumentNullException(nameof(commandRequest), $"{nameof(commandRequest)} is null");
            }

            if (commandRequest.Command == "purge")
            {
                this.Purge(commandRequest.Parameters);
            }
            else
            {
                this.NextHandler.Handle(commandRequest);
            }
        }

        private void Purge(string parameters)
        {
            (int countOfDeletedRecords, int countOfRecords) = this.CabinetService.PurgeDeletedRecords();
            this.consoleWriter.Invoke($"Data file processing is completed: {countOfDeletedRecords} of {countOfRecords} records were purged");
        }
    }
}