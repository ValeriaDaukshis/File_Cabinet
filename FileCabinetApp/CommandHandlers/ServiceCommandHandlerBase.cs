using FileCabinetApp.Service;

namespace FileCabinetApp.CommandHandlers
{
    public abstract class ServiceCommandHandlerBase : CommandHandlerBase
    {
        protected IFileCabinetService fileCabinetService;

        public ServiceCommandHandlerBase(IFileCabinetService fileCabinetService)
        {
            this.fileCabinetService = fileCabinetService;
        }
    }
}