using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using FileCabinetApp.ExceptionClasses;
using FileCabinetApp.Memoization;
using FileCabinetApp.Records;
using FileCabinetApp.Service;

namespace FileCabinetApp.CommandHandlers.ServiceCommandHandlers
{
    /// <summary>
    /// UpdateCommandHandler.
    /// </summary>
    public class UpdateCommandHandler : ServiceCommandHandlerBase
    {
        private readonly IExpressionExtensions expressionExtensions;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommandHandler"/> class.
        /// DeleteCommandHandler constructor.
        /// </summary>
        /// <param name="cabinetService">fileCabinetService.</param>
        /// <param name="expressionExtensions">expressionExtensions.</param>
        public UpdateCommandHandler(IFileCabinetService cabinetService, IExpressionExtensions expressionExtensions)
            : base(cabinetService)
        {
            this.expressionExtensions = expressionExtensions;
        }

        /// <summary>
        /// Handles the specified command request.
        /// </summary>
        /// <param name="commandRequest">The command request.</param>
        /// <exception cref="ArgumentNullException">commandRequest.</exception>
        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is null)
            {
                throw new ArgumentNullException(nameof(commandRequest), $"{nameof(commandRequest)} is null");
            }

            if (commandRequest.Command == "update")
            {
                Cache.Clear();
                this.Update(commandRequest.Parameters);
            }
            else
            {
                this.NextHandler.Handle(commandRequest);
            }
        }

        private static string CreateOutputText(int[] recordsId)
        {
            StringBuilder builder = new StringBuilder();
            if (recordsId.Length == 0)
            {
                return "Records with this parameters not found";
            }

            if (recordsId.Length > 1)
            {
                builder.Append("Records ");
            }
            else
            {
                builder.Append("Record ");
            }

            for (int i = 0; i < recordsId.Length; i++)
            {
                builder.Append($"#{recordsId[i]} ");
            }

            if (recordsId.Length > 1)
            {
                builder.Append("were updated");
            }
            else
            {
                builder.Append("was updated");
            }

            return builder.ToString();
        }

        private static void CheckInputConditionFields(string[] conditionFields)
        {
            if (conditionFields.Length < 2)
            {
                throw new ArgumentException($"{nameof(conditionFields)} Not enough parameters after condition command 'where'. Use 'help' or 'syntax'", nameof(conditionFields));
            }

            if (conditionFields.Length > 2 && (!conditionFields.Contains("and") && !conditionFields.Contains("or")))
            {
                throw new ArgumentException($"{nameof(conditionFields)} parameters after 'where' don't have separator 'and(or)'. Use 'help' or 'syntax'", nameof(conditionFields));
            }
        }

        private static string CheckConditionSeparator(string[] conditionFields)
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

        private static void CheckInputUpdateFields(string[] updatedFields)
        {
            if (updatedFields.Length < 3)
            {
                throw new ArgumentException($"{nameof(updatedFields)} Not enough parameters after condition command 'update'. Use 'help' or 'syntax'", nameof(updatedFields));
            }

            if (updatedFields.Length > 2 && !updatedFields.Contains("set"))
            {
                throw new ArgumentException($"{nameof(updatedFields)} parameters after 'update' don't have separator 'set'. Use 'help' or 'syntax.'", nameof(updatedFields));
            }
        }

        private static Dictionary<string, string> CreateFieldsDictionary(string[] values, string separator)
        {
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

                if (!FieldsCaseDictionary.ContainsKey(key))
                {
                    throw new ArgumentException($"No field with name {nameof(values)}", nameof(values));
                }

                updates.Add(FieldsCaseDictionary[key], values[++counter]);
                counter++;
            }

            return updates;
        }

        private void Update(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                Console.WriteLine("Write information");
                return;
            }

            char[] separators = { '=', ',', ' ' };
            string[] inputs = parameters.Split("where", StringSplitOptions.RemoveEmptyEntries);
            string[] updatedFields = inputs[0].Split(separators, StringSplitOptions.RemoveEmptyEntries);
            string[] conditionFields = inputs[1].Split(separators, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                CheckInputConditionFields(conditionFields);
                CheckInputUpdateFields(updatedFields);

                // finds separator (or/and)
                string conditionSeparator = CheckConditionSeparator(conditionFields);
                Dictionary<string, string> updates = CreateFieldsDictionary(updatedFields, "set");
                Dictionary<string, string> conditions = CreateFieldsDictionary(conditionFields, conditionSeparator);

                List<int> recordsId = new List<int>();

                // finds records that satisfy the condition
                var records = this.expressionExtensions.FindSuitableRecords(conditions.Values.ToArray(), conditions.Keys.ToArray(), conditionSeparator, typeof(FileCabinetRecord)).ToArray();

                for (int i = 0; i < records.Length; i++)
                {
                    CheckInputFields(updates.Keys.ToArray(), updates.Values.ToArray(), records[i], out string firstName, out string lastName, out char gender, out DateTime dateOfBirth, out decimal credit, out short duration);
                    int id = records[i].Id;
                    this.CabinetService.EditRecord(new FileCabinetRecord(id, firstName, lastName, gender, dateOfBirth, credit, duration));
                    recordsId.Add(id);
                }

                Console.WriteLine(CreateOutputText(recordsId.ToArray()));
            }
            catch (FileRecordNotFoundException ex)
            {
                Console.WriteLine($"{ex.Value} was not found");
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}