using System;

namespace FileCabinetApp
{
    /// <summary>
    /// ConsoleWriters.
    /// </summary>
    public class ModelWriters
    {
        /// <summary>
        /// Gets or sets the console writer.
        /// </summary>
        /// <value>
        /// The console writer.
        /// </value>
        public Action<string> LineWriter { get; set; }

        /// <summary>
        /// Gets or sets the writer.
        /// </summary>
        /// <value>
        /// The writer.
        /// </value>
        public Action<string> Writer { get; set; }

        /// <summary>
        /// Gets or sets the console reader.
        /// </summary>
        /// <value>
        /// The console reader.
        /// </value>
        public Func<string> Reader { get; set; }
    }
}