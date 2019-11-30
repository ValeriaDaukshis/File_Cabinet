using System;

using FileCabinetApp.CommandHandlers.Extensions;
using FileCabinetApp.Service;

namespace FileCabinetApp.CommandHandlers.ServiceCommandHandlers
{
    /// <summary>
    ///     InsertCommandHandler.
    /// </summary>
    /// <seealso cref="FileCabinetApp.CommandHandlers.ServiceCommandHandlerBase" />
    public class InsertCommandHandler : ServiceCommandHandlerBase
    {
        private readonly ModelWriters modelWriter;

        /// <summary>
        ///     Initializes a new instance of the <see cref="InsertCommandHandler" /> class.
        /// </summary>
        /// <param name="cabinetService">The file cabinet service.</param>
        /// <param name="modelWriter">console writer.</param>
        public InsertCommandHandler(IFileCabinetService cabinetService, ModelWriters modelWriter)
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
                throw new ArgumentException("Length of values and fields not equal.");
            }
        }

        private void Create(string parameters)
        {
            char[] separators = { '(', ')', ',', ' ' };
            var inputs = parameters.Split("values", StringSplitOptions.RemoveEmptyEntries);

            try
            {
                var fields = inputs[0].Split(separators, StringSplitOptions.RemoveEmptyEntries);
                var values = inputs[1].Split(separators, StringSplitOptions.RemoveEmptyEntries);

                CompareFieldsAndInputArraysLength(fields, values);
                this.CommandHandlersExtensions.ChangeFieldCase(fields);
                CommandHandlersExtensions.DeleteQuotesFromInputValues(values);

                var recordNumber = this.CabinetService.CreateRecord(this.InputValidator.PrintInputFields(fields, values));
                this.modelWriter.LineWriter.Invoke($"Record #{recordNumber} is created.");
            }
            catch (ArgumentNullException ex)
            {
                this.modelWriter.LineWriter.Invoke(ex.Message);
                this.modelWriter.LineWriter.Invoke("Record is not created ");
            }
            catch (ArgumentException ex)
            {
                this.modelWriter.LineWriter.Invoke(ex.Message);
                this.modelWriter.LineWriter.Invoke("Record is not created ");
            }
            catch (IndexOutOfRangeException ex)
            {
                this.modelWriter.LineWriter.Invoke(ex.Message);
                this.modelWriter.LineWriter.Invoke("Record is not created ");
            }
        }
    }
}