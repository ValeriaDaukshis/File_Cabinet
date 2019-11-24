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
        private readonly Action<string> consoleWriter;
        private readonly Action<string> lineWriter;
        private readonly Func<string> consoleReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCommandHandler"/> class.
        /// </summary>
        /// <param name="cabinetService">The file cabinet service.</param>
        /// <param name="consoleWriter">console writer.</param>
        /// <param name="lineWriter">line writer.</param>
        public CreateCommandHandler(IFileCabinetService cabinetService, Action<string> consoleWriter, Action<string> lineWriter, Func<string> consoleReader)
            : base(cabinetService)
        {
            this.consoleWriter = consoleWriter;
            this.lineWriter = lineWriter;
            this.consoleReader = consoleReader;
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
                var recordNumber = this.CabinetService.CreateRecord(this.InputReader.PrintInputFields(this.lineWriter, this.consoleReader));
                this.consoleWriter.Invoke($"Record #{recordNumber} is created.");
            }
            catch (ArgumentNullException ex)
            {
                this.consoleWriter.Invoke(ex.Message);
                this.consoleWriter.Invoke("Record is not created ");
            }
            catch (ArgumentException ex)
            {
                this.consoleWriter.Invoke(ex.Message);
                this.consoleWriter.Invoke("Record is not created ");
            }
        }
    }
}