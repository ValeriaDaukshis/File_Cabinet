using System;
using FileCabinetApp.Service;

namespace FileCabinetApp.CommandHandlers.ServiceCommandHandlers
{
    /// <summary>
    /// StatCommandHandler.
    /// </summary>
    /// <seealso cref="FileCabinetApp.CommandHandlers.ServiceCommandHandlerBase" />
    public class StatCommandHandler : ServiceCommandHandlerBase
    {
        private readonly Action<string> consoleWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatCommandHandler"/> class.
        /// </summary>
        /// <param name="cabinetService">The file cabinet service.</param>
        /// <param name="consoleWriter">console writer.</param>
        public StatCommandHandler(IFileCabinetService cabinetService, Action<string> consoleWriter)
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
            (int purgedRecords, int recordsCount) = this.CabinetService.GetStat();
            this.consoleWriter.Invoke($"{recordsCount} record(s), where {purgedRecords} are purged.");
        }
    }
}