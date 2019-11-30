using System;

namespace FileCabinetApp.CommandHandlers.FunctionalCommandHandlers
{
    /// <summary>
    ///     HelpCommandHandler.
    /// </summary>
    /// <seealso cref="FileCabinetApp.CommandHandlers.CommandHandlerBase" />
    public class HelpCommandHandler : CommandHandlerBase
    {
        private const int CommandHelpIndex = 0;

        private const int DescriptionHelpIndex = 1;

        private const int ExplanationHelpIndex = 2;

        private static string[][] helpMessages;

        private static ModelWriters modelWriter;

        /// <summary>
        ///     Initializes a new instance of the <see cref="HelpCommandHandler" /> class.
        /// </summary>
        /// <param name="help">The help.</param>
        /// <param name="modelWriter">console writer.</param>
        public HelpCommandHandler(string[][] help, ModelWriters modelWriter)
        {
            helpMessages = help;
            HelpCommandHandler.modelWriter = modelWriter;
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

            if (commandRequest.Command == "help")
            {
                PrintHelp(commandRequest.Parameters);
            }
            else
            {
                this.NextHandler.Handle(commandRequest);
            }
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    modelWriter.LineWriter.Invoke(helpMessages[index][ExplanationHelpIndex]);
                }
                else
                {
                    modelWriter.LineWriter.Invoke($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                modelWriter.LineWriter.Invoke("Available commands:");

                foreach (var helpMessage in helpMessages)
                {
                    modelWriter.LineWriter.Invoke($"\t{helpMessage[CommandHelpIndex]}\t- {helpMessage[DescriptionHelpIndex]}");
                }
            }
        }
    }
}