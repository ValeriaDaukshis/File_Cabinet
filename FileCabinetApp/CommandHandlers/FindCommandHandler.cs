using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FileCabinetApp.CommandHandlers
{
    public class FindCommandHandler : CommandHandlerBase
    {
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest.Command == "find")
            {
                Find(commandRequest.Parameters);
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(commandRequest);
            }
        }

        private static void Find(string parameters)
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
                foundResult = Program.fileCabinetService.FindByFirstName(value);
            }
            else if (field == "lastname")
            {
                foundResult = Program.fileCabinetService.FindByLastName(value);
            }
            else if (field == "dateofbirth")
            {
                DateTime.TryParse(value, DateTimeCulture, DateTimeStyles.None, out var date);
                foundResult = Program.fileCabinetService.FindByDateOfBirth(date);
            }

            if (foundResult.Count > 0)
            {
                PrintRecords(foundResult);
            }

            if (foundResult.Count == 0)
            {
                Console.WriteLine($"{value} is not found");
            }
        }
    }
}