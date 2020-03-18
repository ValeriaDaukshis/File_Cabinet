using System;

using FileCabinetApp.Service;

namespace FileCabinetApp.CommandHandlers.ServiceCommandHandlers
{
    /// <summary>
    ///     StatCommandHandler.
    /// </summary>
    /// <seealso cref="ServiceCommandHandlerBase" />
    public class StatCommandHandler : ServiceCommandHandlerBase
    {
        private readonly ModelWriters modelWriter;

        /// <summary>
        ///     Initializes a new instance of the <see cref="StatCommandHandler" /> class.
        /// </summary>
        /// <param name="cabinetService">The file cabinet service.</param>
        /// <param name="modelWriter">console writer.</param>
        public StatCommandHandler(IFileCabinetService cabinetService, ModelWriters modelWriter)
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

            if (commandRequest.Command == "stat")
            {
                this.Stat(commandRequest.Parameters);
            }
            else
            {
                this.NextHandler.Handle(commandRequest);
            }
        }

        private void Stat(string parameters)
        {
            var (purgedRecords, recordsCount) = this.CabinetService.GetStat();
            this.modelWriter.LineWriter.Invoke($"{recordsCount} record(s), where {purgedRecords} are purged.");
        }
    }
}