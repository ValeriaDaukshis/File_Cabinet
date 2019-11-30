using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Security;
using System.Reflection;
using System.Text;
using FileCabinetApp.CommandHandlers.Extensions;
using FileCabinetApp.ExceptionClasses;
using FileCabinetApp.Records;
using FileCabinetApp.Service;

namespace FileCabinetApp.CommandHandlers.ServiceCommandHandlers
{
    /// <summary>
    ///     DeleteCommandHandler.
    /// </summary>
    public class DeleteCommandHandler : ServiceCommandHandlerBase
    {
        private readonly IExpressionExtensions expressionExtensions;

        private readonly ModelWriters modelWriter;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DeleteCommandHandler" /> class.
        ///     DeleteCommandHandler constructor.
        /// </summary>
        /// <param name="cabinetService">fileCabinetService.</param>
        /// <param name="expressionExtensions">expressionExtensions.</param>
        /// <param name="modelWriter">console writer.</param>
        public DeleteCommandHandler(IFileCabinetService cabinetService, IExpressionExtensions expressionExtensions, ModelWriters modelWriter) : base(cabinetService)
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

            if (commandRequest.Command == "delete")
            {
                Cache.Clear();
                this.Delete(commandRequest.Parameters);
            }
            else
            {
                this.NextHandler.Handle(commandRequest);
            }
        }

        private static string ChangeFieldCase(string value)
        {
            value = value.ToLower(Culture);
            if (!FieldsCaseDictionary.ContainsKey(value))
            {
                throw new ArgumentException($"No field with name {nameof(value)}");
            }

            value = FieldsCaseDictionary[value];
            return value;
        }

        private static string CreateOutputText(int[] recordsId)
        {
            if (recordsId.Length == 0)
            {
                return "No records found";
            }

            var builder = new StringBuilder();

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
                builder.Append(" are deleted");
            }
            else
            {
                builder.Append("is deleted");
            }

            return builder.ToString();
        }

        private void Delete(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                this.modelWriter.LineWriter.Invoke("Write a record number");
                return;
            }

            char[] separator = { ' ', '=' };
            var inputs = parameters.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            if (inputs.Length < 3)
            {
                this.modelWriter.LineWriter.Invoke("Not enough parameters after command 'delete'. Use 'help' or 'syntax'");
                return;
            }

            if (inputs.Length > 3)
            {
                this.modelWriter.LineWriter.Invoke("A lot of parameters after command 'delete'. Use 'help' or 'syntax'");
                return;
            }

            var fieldName = ChangeFieldCase(inputs[1]);
            var parameter = inputs[2];
            if (parameter[0] == '\'' || parameter[0] == '"')
            {
                parameter = parameter.Substring(1, parameter.Length - 2);
            }

            try
            {
                var recordsId = new List<int>();
                var records = this.expressionExtensions.FindSuitableRecords(parameter, fieldName, typeof(FileCabinetRecord)).ToArray();

                for (var i = 0; i < records.Length; i++)
                {
                    recordsId.Add(this.CabinetService.RemoveRecord(records[i]));
                }

                this.modelWriter.LineWriter.Invoke(CreateOutputText(recordsId.ToArray()));
            }
            catch (FileRecordNotFoundException ex)
            {
                this.modelWriter.LineWriter.Invoke($"{ex.Value} was not found");
                this.modelWriter.LineWriter.Invoke($"Record #{parameters} was not deleted ");
            }
            catch (FormatException ex)
            {
                this.modelWriter.LineWriter.Invoke(ex.Message);
                this.modelWriter.LineWriter.Invoke($"Record #{parameters} was not deleted ");
            }
            catch (ArgumentNullException ex)
            {
                this.modelWriter.LineWriter.Invoke(ex.Message);
                this.modelWriter.LineWriter.Invoke($"Record #{parameters} was not deleted ");
            }
            catch (ArgumentException ex)
            {
                this.modelWriter.LineWriter.Invoke(ex.Message);
                this.modelWriter.LineWriter.Invoke($"Record #{parameters} was not deleted");
            }
        }
    }
}