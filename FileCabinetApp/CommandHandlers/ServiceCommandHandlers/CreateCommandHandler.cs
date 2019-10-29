using System;
using FileCabinetApp.Service;

namespace FileCabinetApp.CommandHandlers.ServiceCommandHandlers
{
    public class CreateCommandHandler : ServiceCommandHandlerBase
    {
        public CreateCommandHandler(IFileCabinetService fileCabinetService) : base(fileCabinetService)
        {
        }

        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest.Command == "create")
            {
                this.Create(commandRequest.Parameters);
            }
            else if (this.nextHandler != null)
            {
                this.nextHandler.Handle(commandRequest);
            }
        }

        private void Create(string parameters)
        {
            PrintInputFields(out string firstName, out string lastName, out char gender, out DateTime dateOfBirth, out decimal credit, out short duration);
            try
            {
                var recordNumber =
                    this.fileCabinetService.CreateRecord(new FileCabinetRecord(firstName, lastName, gender, dateOfBirth, credit, duration));
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