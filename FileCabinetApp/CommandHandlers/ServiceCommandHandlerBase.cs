using FileCabinetApp.Service;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// ServiceCommandHandlerBase.
    /// </summary>
    /// <seealso cref="FileCabinetApp.CommandHandlers.CommandHandlerBase" />
    public abstract class ServiceCommandHandlerBase : CommandHandlerBase
    {
        /// <summary>
        /// IFileCabinetService.
        /// </summary>
        protected readonly IFileCabinetService FileCabinetService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCommandHandlerBase"/> class.
        /// </summary>
        /// <param name="fileCabinetService">The file cabinet service.</param>
        protected ServiceCommandHandlerBase(IFileCabinetService fileCabinetService)
        {
            this.FileCabinetService = fileCabinetService;
        }
    }
}