using System;

namespace FileCabinetApp
{
    /// <summary>
    /// ConsoleWriters.
    /// </summary>
    public class ConsoleWriters
    {
        /// <summary>
        /// Gets the console writer.
        /// </summary>
        /// <value>
        /// The console writer.
        /// </value>
        public Action<string> LineWriter { get; } = Console.WriteLine;

        /// <summary>
        /// Gets the writer.
        /// </summary>
        /// <value>
        /// The writer.
        /// </value>
        public Action<string> Writer { get; } = Console.Write;

        /// <summary>
        /// Gets the console reader.
        /// </summary>
        /// <value>
        /// The console reader.
        /// </value>
        public Func<string> Reader { get; } = Console.ReadLine;
    }
}