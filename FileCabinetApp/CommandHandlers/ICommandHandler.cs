namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// ICommandHandler.
    /// </summary>
    public interface ICommandHandler
    {
        /// <summary>
        /// Sets the next.
        /// </summary>
        /// <param name="commandHandler">The command handler.</param>
        /// <returns>ICommandHandler.</returns>
        ICommandHandler SetNext(ICommandHandler commandHandler);

        /// <summary>
        /// Handles the specified command request.
        /// </summary>
        /// <param name="commandRequest">The command request.</param>
        void Handle(AppCommandRequest commandRequest);
    }
}