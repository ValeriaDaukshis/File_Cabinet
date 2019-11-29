using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace FileCabinetApp.CommandHandlers.Extensions
{
    /// <summary>
    /// CommandHandlersExpressions.
    /// </summary>
    public class CommandHandlersExtensions
    {
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;
        private readonly Dictionary<string, string> fieldsCaseDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandlersExtensions"/> class.
        /// </summary>
        /// <param name="fieldsCaseDictionary">The fields case dictionary.</param>
        public CommandHandlersExtensions(Dictionary<string, string> fieldsCaseDictionary)
        {
            this.fieldsCaseDictionary = fieldsCaseDictionary;
        }

        /// <summary>
        /// Deletes the quotes from input values.
        /// </summary>
        /// <param name="values">The values.</param>
        public static void DeleteQuotesFromInputValues(string[] values)
        {
            if (values is null)
            {
                throw new ArgumentNullException(nameof(values), $"{nameof(values)} is null");
            }

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i][0] == '\'' || values[i][0] == '"')
                {
                    values[i] = values[i].Substring(1, values[i].Length - 2);
                }
            }
        }

        /// <summary>
        /// Imports the export parameters spliter.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="fileFormat">The file format.</param>
        /// <param name="path">The path.</param>
        /// <param name="commandName">The command name.</param>
        /// <returns>true if params are valid.</returns>
        public static bool ImportExportParametersSpliter(string parameters, out string fileFormat, out string path, string commandName)
        {
            fileFormat = string.Empty;
            path = string.Empty;
            if (string.IsNullOrEmpty(parameters))
            {
                Console.WriteLine($"No parameters after command '{commandName}'");
                return false;
            }

            string[] inputParameters = parameters.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

            if (inputParameters.Length < 2)
            {
                Console.WriteLine($"Not enough parameters after command '{commandName}'");
                return false;
            }

            fileFormat = inputParameters[0].ToLower(Culture);
            path = inputParameters[1];
            return true;
        }

        /// <summary>
        /// Finds the condition separator.
        /// </summary>
        /// <param name="conditionFields">The condition fields.</param>
        /// <returns>separator.</returns>
        /// <exception cref="ArgumentException">conditionFields.</exception>
        public static string FindConditionSeparator(string[] conditionFields)
        {
            if (conditionFields.Contains("and") && conditionFields.Contains("or"))
            {
                throw new ArgumentException($"{nameof(conditionFields)} request can not contains separator 'and', 'or' together. Use 'help' or 'syntax'", nameof(conditionFields));
            }

            if (conditionFields.Contains("and"))
            {
                return "and";
            }

            if (conditionFields.Contains("or"))
            {
                return "or";
            }

            return string.Empty;
        }

        /// <summary>
        /// Creates the dictionary of fields.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="separator">The separator.</param>
        /// <returns>dictionary of values.</returns>
        /// <exception cref="ArgumentNullException">values.</exception>
        /// <exception cref="ArgumentException">No field with name {nameof(values)} - values.</exception>
        public Dictionary<string, string> CreateDictionaryOfFields(string[] values, string separator)
        {
            if (values is null)
            {
                throw new ArgumentNullException(nameof(values), $"{nameof(values)} is null");
            }

            Dictionary<string, string> updates = new Dictionary<string, string>();
            DeleteQuotesFromInputValues(values);

            int counter = 0;
            while (counter < values.Length)
            {
                if (values[counter] == separator)
                {
                    counter++;
                    continue;
                }

                var key = values[counter].ToLower(Culture);

                if (!this.fieldsCaseDictionary.ContainsKey(key))
                {
                    throw new ArgumentException($"No field with name {values.GetValue(counter)}");
                }

                updates.Add(this.fieldsCaseDictionary[key], values[++counter]);
                counter++;
            }

            return updates;
        }

        /// <summary>
        /// Changes the field case.
        /// </summary>
        /// <param name="printedFields">The printed fields.</param>
        /// <exception cref="ArgumentNullException">printedFields.</exception>
        /// <exception cref="ArgumentException">No field with name {nameof(printedFields)} - printedFields.</exception>
        public void ChangeFieldCase(string[] printedFields)
        {
            if (printedFields is null)
            {
                throw new ArgumentNullException(nameof(printedFields), $"{nameof(printedFields)} is null");
            }

            for (int i = 0; i < printedFields.Length; i++)
            {
                var key = printedFields[i].ToLower(Culture);

                if (!this.fieldsCaseDictionary.ContainsKey(key))
                {
                    throw new ArgumentException($"No field with name {printedFields.GetValue(i)}");
                }

                printedFields[i] = this.fieldsCaseDictionary[key];
            }
        }
    }
}
