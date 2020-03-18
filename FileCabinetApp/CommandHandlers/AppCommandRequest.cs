namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    ///     AppCommandRequest.
    /// </summary>
    public class AppCommandRequest
    {
        /// <summary>
        ///     Gets or sets the command.
        /// </summary>
        /// <value>
        ///     The command.
        /// </value>
        public string Command { get; set; }

        /// <summary>
        ///     Gets or sets the parameters.
        /// </summary>
        /// <value>
        ///     The parameters.
        /// </value>
        public string Parameters { get; set; }
    }
}