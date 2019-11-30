using System;

using FileCabinetApp.Service;

namespace FileCabinetApp.CommandHandlers.ServiceCommandHandlers
{
    /// <summary>
    ///     PurgeCommandHandler.
    /// </summary>
    /// <seealso cref="FileCabinetApp.CommandHandlers.ServiceCommandHandlerBase" />
    public class PurgeCommandHandler : ServiceCommandHandlerBase
    {
        private readonly ModelWriters modelWriter;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PurgeCommandHandler" /> class.
        /// </summary>
        /// <param name="cabinetService">The file cabinet service.</param>
        /// <param name="modelWriter">console writer.</param>
        public PurgeCommandHandler(IFileCabinetService cabinetService, ModelWriters modelWriter)
            : base(cabinetService)
        {
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
            var (countOfDeletedRecords, countOfRecords) = this.CabinetService.PurgeDeletedRecords();
            this.modelWriter.LineWriter.Invoke($"Data file processing is completed: {countOfDeletedRecords} of {countOfRecords} records were purged");
        }
    }
}