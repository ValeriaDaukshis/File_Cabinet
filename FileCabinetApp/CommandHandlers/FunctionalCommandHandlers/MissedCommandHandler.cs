using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileCabinetApp.CommandHandlers.FunctionalCommandHandlers
{
    /// <summary>
    ///     MissedCommandHandler.
    /// </summary>
    /// <seealso cref="CommandHandlerBase" />
    public class MissedCommandHandler : CommandHandlerBase
    {
        private static string[] commands;

        private static ModelWriters modelWriter;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MissedCommandHandler" /> class.
        /// </summary>
        /// <param name="commands">The commands.</param>
        /// <param name="modelWriter">console writer.</param>
        public MissedCommandHandler(string[] commands, ModelWriters modelWriter)
        {
            MissedCommandHandler.commands = commands;
            MissedCommandHandler.modelWriter = modelWriter;
        }

        /// <summary>
        ///     Handles the specified command request.
        /// </summary>
        /// <param name="commandRequest">The command request.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is null)
            {
                throw new ArgumentNullException(nameof(commandRequest), $"{nameof(commandRequest)} is null");
            }

            PrintMissedCommandInfo(commandRequest.Command);
        }

        private static IEnumerable<string> FindCommands(string command)
        {
            IEnumerable<string> mostSimilarCommands = new List<string>();
            var parameter = command;
            while (!mostSimilarCommands.Any() && parameter.Length > 0)
            {
                var parameter1 = parameter;
                mostSimilarCommands = from n in commands where n.StartsWith(parameter1, StringComparison.InvariantCulture) select n;
                parameter = parameter.Substring(0, parameter.Length - 1);
            }

            return mostSimilarCommands;
        }

        private static string PrintCommands(IEnumerable<string> printedCommands, string command)
        {
            var builder = new StringBuilder();
            builder.Append($"There is no '{command}' command.\n");
            if (!printedCommands.Any())
            {
                builder.Append("Use 'help' or 'syntax'.");
                return builder.ToString();
            }

            if (printedCommands.Count() > 1)
            {
                builder.Append("The most similar commands are\n");
            }
            else
            {
                builder.Append("The most similar command is\n");
            }

            foreach (var value in printedCommands)
            {
                builder.Append($"\t {value}\n");
            }

            return builder.ToString();
        }

        private static void PrintMissedCommandInfo(string command)
        {
            var mostSimilarCommands = FindCommands(command);
            modelWriter.LineWriter.Invoke(PrintCommands(mostSimilarCommands, command));
        }
    }
}