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
    /// DeleteCommandHandler.
    /// </summary>
    public class DeleteCommandHandler : ServiceCommandHandlerBase
    {
        private readonly IExpressionExtensions expressionExtensions;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCommandHandler"/> class.
        /// DeleteCommandHandler constructor.
        /// </summary>
        /// <param name="cabinetService">fileCabinetService.</param>
        /// <param name="expressionExtensions">expressionExtensions.</param>
        public DeleteCommandHandler(IFileCabinetService cabinetService, IExpressionExtensions expressionExtensions)
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

        private static string CreateOutputText(int[] recordsId)
        {
            if (recordsId.Length == 0)
            {
                return "No records found";
            }

            StringBuilder builder = new StringBuilder();

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
                builder.Append(" are deleted");
            }
            else
            {
                builder.Append("is deleted");
            }

            return builder.ToString();
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

        private void Delete(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                Console.WriteLine("Write a record number");
                return;
            }

            char[] separator = { ' ', '=' };
            string[] inputs = parameters.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            if (inputs.Length < 3)
            {
                Console.WriteLine("Not enough parameters after command 'delete'. Use 'help' or 'syntax'");
                return;
            }

            if (inputs.Length > 3)
            {
                Console.WriteLine("A lot of parameters after command 'delete'. Use 'help' or 'syntax'");
                return;
            }

            string fieldName = ChangeFieldCase(inputs[1]);
            string parameter = inputs[2];
            if (parameter[0] == '\'' || parameter[0] == '"')
            {
                parameter = parameter.Substring(1, parameter.Length - 2);
            }

            try
            {
                List<int> recordsId = new List<int>();
                var records = this.expressionExtensions
                    .FindSuitableRecords(parameter, fieldName, typeof(FileCabinetRecord)).ToArray();

                for (int i = 0; i < records.Length; i++)
                {
                    recordsId.Add(this.CabinetService.RemoveRecord(records[i]));
                }

                Console.WriteLine(CreateOutputText(recordsId.ToArray()));
            }
            catch (FileRecordNotFoundException ex)
            {
                Console.WriteLine($"{ex.Value} was not found");
                Console.WriteLine($"Record #{parameters} was not deleted ");
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Record #{parameters} was not deleted ");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Record #{parameters} was not deleted");
            }
        }
    }
}