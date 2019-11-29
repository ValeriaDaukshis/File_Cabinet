using System;
using System.Linq;
using FileCabinetApp.Records;
using FileCabinetApp.Service;

namespace FileCabinetApp.CommandHandlers.ServiceCommandHandlers
{
    /// <summary>
    /// CreateCommandHandler.
    /// </summary>
    /// <seealso cref="FileCabinetApp.CommandHandlers.ServiceCommandHandlerBase" />
    public class CreateCommandHandler : ServiceCommandHandlerBase
    {
        private readonly ModelWriters modelWriters;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCommandHandler"/> class.
        /// </summary>
        /// <param name="cabinetService">The file cabinet service.</param>
        /// <param name="modelWriters">console writer.</param>
        public CreateCommandHandler(IFileCabinetService cabinetService, ModelWriters modelWriters)
            : base(cabinetService)
        {
            this.modelWriters = modelWriters;
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

            if (commandRequest.Command == "create")
            {
                this.Create(commandRequest.Parameters);
            }
            else
            {
                this.NextHandler.Handle(commandRequest);
            }
        }

        private void Create(string parameters)
        {
            try
            {
                var recordNumber = this.CabinetService.CreateRecord(this.InputReader.PrintInputFields(this.modelWriters));
                this.modelWriters.LineWriter.Invoke($"Record #{recordNumber} is created.");
            }
            catch (ArgumentNullException ex)
            {
                this.modelWriters.LineWriter.Invoke(ex.Message);
                this.modelWriters.LineWriter.Invoke("Record is not created ");
            }
            catch (ArgumentException ex)
            {
                this.modelWriters.LineWriter.Invoke(ex.Message);
                this.modelWriters.LineWriter.Invoke("Record is not created ");
            }
        }
    }
}