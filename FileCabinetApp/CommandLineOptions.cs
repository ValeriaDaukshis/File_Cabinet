using CommandLine;

namespace FileCabinetApp
{
    /// <summary>
    ///     CommandLineOptions.
    /// </summary>
    public class CommandLineOptions
    {
        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="CommandLineOptions" /> is logger.
        /// </summary>
        /// <value>
        ///     <c>true</c> if logger; otherwise, <c>false</c>.
        /// </value>
        [Option('l', "use-logger", Required = false, HelpText = "Logging methods")]
        public bool Logger { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [stop watcher].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [stop watcher]; otherwise, <c>false</c>.
        /// </value>
        [Option('u', "use-stopwatch", Required = false, HelpText = "Use methods measuring")]
        public bool StopWatcher { get; set; }

        /// <summary>
        ///     Gets or sets the storage.
        /// </summary>
        /// <value>
        ///     The storage.
        /// </value>
        [Option('s', "storage", Required = false, HelpText = "Set storage place (memory/file)")]
        public string Storage { get; set; }

        /// <summary>
        ///     Gets or sets the validation rules.
        /// </summary>
        /// <value>
        ///     The validation rules.
        /// </value>
        [Option('v', "validation-rules", Required = false, HelpText = "Set validation rules(default/custom)")]
        public string ValidationRules { get; set; }
    }
}