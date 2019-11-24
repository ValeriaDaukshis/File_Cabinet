using System;
using FileCabinetApp.Memoization;
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
                Cache.Clear();
                this.Create(commandRequest.Parameters);
            }
            else
            {
                this.NextHandler.Handle(commandRequest);
            }
        }

        private static void CompareFieldsAndInputArraysLength(string[] fields, string[] values)
        {
            if (fields.Length != values.Length)
            {
                throw new ArgumentException($"Length of values and fields not equal.");
            }
        }

        private void Create(string parameters)
        {
            char[] separators = { '(', ')', ',', ' ' };
            string[] inputs = parameters.Split("values", StringSplitOptions.RemoveEmptyEntries);

            try
            {
                string[] fields = inputs[0].Split(separators, StringSplitOptions.RemoveEmptyEntries);
                string[] values = inputs[1].Split(separators, StringSplitOptions.RemoveEmptyEntries);

                CompareFieldsAndInputArraysLength(fields, values);
                this.CommandHandlersExpressions.ChangeFieldCase(fields);
                CommandHandlersExpressions.DeleteQuotesFromInputValues(values);
                this.PrintInputFields(fields, values, out string firstName, out string lastName, out char gender,
                    out DateTime dateOfBirth, out decimal credit, out short duration);

                var recordNumber =
                    this.CabinetService.CreateRecord(new FileCabinetRecord(firstName, lastName, gender, dateOfBirth,
                        credit, duration));
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
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Record is not created ");
            }
        }
    }
}