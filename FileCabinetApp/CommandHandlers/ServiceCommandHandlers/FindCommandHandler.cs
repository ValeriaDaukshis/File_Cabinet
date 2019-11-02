using System;
using System.Collections.ObjectModel;
using System.Globalization;
using FileCabinetApp.CommandHandlers.Printer;
using FileCabinetApp.Records;
using FileCabinetApp.Service;

namespace FileCabinetApp.CommandHandlers.ServiceCommandHandlers
{
    /// <summary>
    /// FindCommandHandler.
    /// </summary>
    /// <seealso cref="FileCabinetApp.CommandHandlers.ServiceCommandHandlerBase" />
    public class FindCommandHandler : ServiceCommandHandlerBase
    {
        private readonly IRecordPrinter printer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">The file cabinet service.</param>
        /// <param name="printer">The printer.</param>
        public FindCommandHandler(IFileCabinetService fileCabinetService, IRecordPrinter printer)
            : base(fileCabinetService)
        {
            this.printer = printer;
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

            if (commandRequest.Command == "find")
            {
                this.Find(commandRequest.Parameters);
            }
            else
            {
                this.NextHandler.Handle(commandRequest);
            }
        }

        private void Find(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                Console.WriteLine("No parameters after command 'find'");
                return;
            }

            parameters = parameters.ToLower(Culture);
            string[] inputParameters = parameters.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (inputParameters.Length < 2)
            {
                Console.WriteLine("Not enough parameters after command 'find'");
                return;
            }

            string field = inputParameters[0];
            string value = inputParameters[1];

            if (value[0] == '"')
            {
                value = value.Substring(1, value.Length - 2);
            }

            ReadOnlyCollection<FileCabinetRecord> foundResult = new ReadOnlyCollection<FileCabinetRecord>(Array.Empty<FileCabinetRecord>());

            if (field == "firstname")
            {
                foundResult = this.FileCabinetService.FindByFirstName(value);
            }
            else if (field == "lastname")
            {
                foundResult = this.FileCabinetService.FindByLastName(value);
            }
            else if (field == "dateofbirth")
            {
                DateTime.TryParse(value, DateTimeCulture, DateTimeStyles.None, out var date);
                foundResult = this.FileCabinetService.FindByDateOfBirth(date);
            }

            if (foundResult.Count > 0)
            {
                this.printer.Print(foundResult);
            }

            if (foundResult.Count == 0)
            {
                Console.WriteLine($"{value} is not found");
            }
        }
    }
}