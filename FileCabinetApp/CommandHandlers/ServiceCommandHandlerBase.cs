using FileCabinetApp.Service;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    ///     ServiceCommandHandlerBase.
    /// </summary>
    /// <seealso cref="CommandHandlerBase" />
    public abstract class ServiceCommandHandlerBase : CommandHandlerBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ServiceCommandHandlerBase" /> class.
        /// </summary>
        /// <param name="cabinetService">The file cabinet service.</param>
        protected ServiceCommandHandlerBase(IFileCabinetService cabinetService)
        {
            this.CabinetService = cabinetService;
        }

        /// <summary>
        ///     Gets or sets iFileCabinetService.
        /// </summary>
        /// <value>
        ///     IFileCabinetService.
        /// </value>
        protected IFileCabinetService CabinetService { get; set; }
    }
}