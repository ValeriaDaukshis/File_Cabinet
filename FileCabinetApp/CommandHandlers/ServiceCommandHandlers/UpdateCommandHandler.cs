using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FileCabinetApp.CommandHandlers.Extensions;
using FileCabinetApp.ExceptionClasses;
using FileCabinetApp.Records;
using FileCabinetApp.Service;

namespace FileCabinetApp.CommandHandlers.ServiceCommandHandlers
{
    /// <summary>
    ///     UpdateCommandHandler.
    /// </summary>
    public class UpdateCommandHandler : ServiceCommandHandlerBase
    {
        private readonly IExpressionExtensions expressionExtensions;

        private readonly ModelWriters modelWriter;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateCommandHandler" /> class.
        ///     DeleteCommandHandler constructor.
        /// </summary>
        /// <param name="cabinetService">fileCabinetService.</param>
        /// <param name="expressionExtensions">expressionExtensions.</param>
        /// <param name="modelWriter">console writer.</param>
        public UpdateCommandHandler(IFileCabinetService cabinetService, IExpressionExtensions expressionExtensions, ModelWriters modelWriter)
            : base(cabinetService)
        {
            this.expressionExtensions = expressionExtensions;
            this.modelWriter = modelWriter;
        }

        /// <summary>
        ///     Handles the specified command request.
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

        private static void CheckConditionFieldsInput(string[] conditionFields)
        {
            if (conditionFields.Length < 2)
            {
                throw new ArgumentException($"{nameof(conditionFields)} Not enough parameters after condition command 'where'. Use 'help' or 'syntax'");
            }

            if (conditionFields.Length > 2 && !conditionFields.Contains("and") && !conditionFields.Contains("or"))
            {
                throw new ArgumentException($"{nameof(conditionFields)} parameters after 'where' don't have separator 'and(or)'. Use 'help' or 'syntax'");
            }
        }

        private static void CheckUpdateFieldsInput(string[] updatedFields)
        {
            if (updatedFields.Length < 3)
            {
                throw new ArgumentException($"{nameof(updatedFields)} Not enough parameters after condition command 'update'. Use 'help' or 'syntax'");
            }

            if (updatedFields.Length > 2 && !updatedFields.Contains("set"))
            {
                throw new ArgumentException($"{nameof(updatedFields)} parameters after 'update' don't have separator 'set'. Use 'help' or 'syntax.'");
            }
        }

        private static void CheckUpdateId(string oldId, string newId)
        {
            if (oldId != newId)
            {
                throw new ArgumentException("Can't update record id.");
            }
        }

        private static string CreateOutputText(int[] recordsId)
        {
            var builder = new StringBuilder();
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

            for (var i = 0; i < recordsId.Length; i++)
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

        private void Update(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                this.modelWriter.LineWriter.Invoke("Write information");
                return;
            }

            char[] separators = { '=', ',', ' ' };
            var inputs = parameters.Split("where", StringSplitOptions.RemoveEmptyEntries);
            var updatedFields = inputs[0].Split(separators, StringSplitOptions.RemoveEmptyEntries);
            var conditionFields = inputs[1].Split(separators, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                CheckConditionFieldsInput(conditionFields);
                CheckUpdateFieldsInput(updatedFields);

                // finds separator (or/and)
                var conditionSeparator = CommandHandlersExtensions.FindConditionSeparator(conditionFields);
                var updates = this.CommandHandlersExtensions.CreateDictionaryOfFields(updatedFields, "set");
                var conditions = this.CommandHandlersExtensions.CreateDictionaryOfFields(conditionFields, conditionSeparator);

                var recordsId = new List<int>();

                // finds records that satisfy the condition
                var records = this.expressionExtensions.FindSuitableRecords(conditions.Values.ToArray(), conditions.Keys.ToArray(), conditionSeparator, typeof(FileCabinetRecord)).ToArray();

                for (var i = 0; i < records.Length; i++)
                {
                    var record = this.InputValidator.CheckInputFields(updates.Keys.ToArray(), updates.Values.ToArray(), records[i]);
                    var id = records[i].Id;
                    if (updates.ContainsKey("Id"))
                    {
                        var newId = updates["Id"];
                        CheckUpdateId(id.ToString(Culture), newId);
                    }

                    record.Id = id;

                    this.CabinetService.EditRecord(record);
                    recordsId.Add(id);
                }

                this.modelWriter.LineWriter.Invoke(CreateOutputText(recordsId.ToArray()));
            }
            catch (FileRecordNotFoundException ex)
            {
                this.modelWriter.LineWriter.Invoke($"{ex.Value} was not found");
            }
            catch (ArgumentNullException ex)
            {
                this.modelWriter.LineWriter.Invoke(ex.Message);
            }
            catch (ArgumentException ex)
            {
                this.modelWriter.LineWriter.Invoke(ex.Message);
            }
        }
    }
}