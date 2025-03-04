﻿using System;

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
        private static ModelWriters modelWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntaxCommandHandler"/> class.
        /// </summary>
        /// <param name="help">The help.</param>
        /// <param name="modelWriter">console writer.</param>
        public SyntaxCommandHandler(string[][] help, ModelWriters modelWriter)
        {
            helpMessages = help;
            SyntaxCommandHandler.modelWriter = modelWriter;
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
                    modelWriter.LineWriter.Invoke(helpMessages[index][SyntaxHelpIndex]);
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
                    modelWriter.LineWriter.Invoke($"\t{helpMessage[CommandHelpIndex]}\t- {helpMessage[SyntaxHelpIndex]}");
                }
            }
        }
    }
}