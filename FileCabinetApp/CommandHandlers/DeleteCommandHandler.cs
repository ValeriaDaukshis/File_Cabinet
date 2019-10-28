using System;
using System.Linq;
using FileCabinetApp.ExceptionClasses;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// CommandHandler.
    /// </summary>
    /// <seealso cref="FileCabinetApp.CommandHandlers.CommandHandlerBase" />
    public class DeleteCommandHandler : CommandHandlerBase
    {

        /// <summary>
        /// Handles the specified command request.
        /// </summary>
        /// <param name="commandRequest">The command request.</param>
        /// <exception cref="ArgumentNullException">commandRequest.</exception>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest.Command == "delete")
            {
                Delete(commandRequest.Parameters);
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(commandRequest);
            }
        }

        private static void Delete(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                Console.WriteLine("Write a record number");
                return;
            }

            if (!int.TryParse(parameters.Trim(), out var id))
            {
                Console.WriteLine($"#{parameters} record is not found");
                return;
            }

            try
            {
                if (RecordIdValidator.TryGetRecordId(id))
                {
                    var records = Program.fileCabinetService.GetRecords().ToList();
                    var record = records.Find(x => x.Id == id);
                    Program.fileCabinetService.RemoveRecord(record);
                    Console.WriteLine($"Record #{parameters} was deleted");
                }
            }
            catch (FileRecordNotFoundException ex)
            {
                Console.WriteLine($"{ex.Value} was not found");
                Console.WriteLine($"Record #{parameters} was not deleted ");
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Record #{parameters} was not deleted ");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Record #{parameters} was not deleted");
            }
        }
    }
}