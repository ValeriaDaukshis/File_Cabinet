using CommandLine;

namespace FileCabinetApp
{
    /// <summary>
    /// CommandLineOptions.
    /// </summary>
    public class CommandLineOptions
    {
        /// <summary>
        /// Gets or sets the validation rules.
        /// </summary>
        /// <value>
        /// The validation rules.
        /// </value>
        [Option('v', "validation-rules", Required = false, HelpText = "set validation rules(default/custom)")]
        public string ValidationRules { get; set; }

        /// <summary>
        /// Gets or sets the storage.
        /// </summary>
        /// <value>
        /// The storage.
        /// </value>
        [Option('s', "storage", Required = false, HelpText = "Set storage place (memory/file)")]
        public string Storage { get; set; }
    }
}