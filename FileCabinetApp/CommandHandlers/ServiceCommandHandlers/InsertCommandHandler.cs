using System;
using FileCabinetApp.Records;
using FileCabinetApp.Service;

namespace FileCabinetApp.CommandHandlers.ServiceCommandHandlers
{
    /// <summary>
    /// InsertCommandHandler.
    /// </summary>
    /// <seealso cref="FileCabinetApp.CommandHandlers.ServiceCommandHandlerBase" />
    public class InsertCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InsertCommandHandler"/> class.
        /// </summary>
        /// <param name="cabinetService">The file cabinet service.</param>
        public InsertCommandHandler(IFileCabinetService cabinetService)
            : base(cabinetService)
        {
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

            if (commandRequest.Command == "insert")
            {
                this.Create(commandRequest.Parameters);
            }
            else
            {
                this.NextHandler.Handle(commandRequest);
            }
        }

        private static void CorrectValuesInput(string[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i][0] == '\'' || values[i][0] == '"')
                {
                    values[i] = values[i].Substring(1, values[i].Length - 2);
                }
            }
        }

        private static void ChangeFieldCase(string[] fields)
        {
            for (int i = 0; i < fields.Length; i++)
            {
                var value = fields[i].ToLower(Culture);
                if (!FieldsCaseDictionary.ContainsKey(value))
                {
                    throw new ArgumentException($"No field with name {nameof(fields)}", nameof(fields));
                }

                fields[i] = FieldsCaseDictionary[value];
            }
        }

        private void Create(string parameters)
        {
            char[] separators = { '(', ')', ',', ' ' };
            string[] inputs = parameters.Split("values", StringSplitOptions.RemoveEmptyEntries);
            string[] fields = inputs[0].Split(separators, StringSplitOptions.RemoveEmptyEntries);
            ChangeFieldCase(fields);
            string[] values = inputs[1].Split(separators, StringSplitOptions.RemoveEmptyEntries);

            CorrectValuesInput(values);

            try
            {
                PrintInputFields(fields, values, out string firstName, out string lastName, out char gender, out DateTime dateOfBirth, out decimal credit, out short duration);

                var recordNumber =
                    this.CabinetService.CreateRecord(new FileCabinetRecord(firstName, lastName, gender, dateOfBirth, credit, duration));
                Console.WriteLine($"Record #{recordNumber} is created.");
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Record is not created ");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Record is not created ");
            }
        }
    }
}