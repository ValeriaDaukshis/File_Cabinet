using System;

namespace FileCabinetApp.CommandHandlers.FunctionalCommandHandlers
{
    /// <summary>
    /// SyntaxCommandHandler.
    /// </summary>
    public class SyntaxCommandHandler : CommandHandlerBase
    {
        private const int CommandHelpIndex = 0;
        private const int SyntaxHelpIndex = 3;
        private static string[][] helpMessages;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntaxCommandHandler"/> class.
        /// </summary>
        /// <param name="help">The help.</param>
        public SyntaxCommandHandler(string[][] help)
        {
            helpMessages = help;
        }

        /// <summary>
        /// Handles the specified command request.
        /// </summary>
        /// <param name="commandRequest">The command request.</param>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is null)
            {
                throw new ArgumentNullException(nameof(commandRequest), $"{nameof(commandRequest)} is null");
            }

            if (commandRequest.Command == "syntax")
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
                    Console.WriteLine(helpMessages[index][SyntaxHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in helpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[CommandHelpIndex], helpMessage[SyntaxHelpIndex]);
                }
            }

            Console.WriteLine();
        }
    }
}