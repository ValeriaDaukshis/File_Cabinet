using System;
using FileCabinetApp.ExceptionClasses;

namespace FileCabinetApp.CommandHandlers
{
    public class EditCommandHandler : CommandHandlerBase
    {
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest.Command == "edit")
            {
                Edit(commandRequest.Parameters);
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(commandRequest);
            }
        }

        private static void Edit(string parameters)
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
                    PrintInputFields(out string firstName, out string lastName, out char gender, out DateTime dateOfBirth, out decimal credit, out short duration);
                    FileCabinetRecord record = new FileCabinetRecord(id, firstName, lastName, gender, dateOfBirth, credit, duration);
                    Program.fileCabinetService.EditRecord(record);
                    Console.WriteLine($"Record #{parameters} is updated");
                }
            }
            catch (FileRecordNotFoundException ex)
            {
                Console.WriteLine($"{ex.Value} was not found");
                Console.WriteLine($"Record #{parameters} was not updated ");
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Record #{parameters} was not updated ");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Record #{parameters} was not updated");
            }
        }
    }
}