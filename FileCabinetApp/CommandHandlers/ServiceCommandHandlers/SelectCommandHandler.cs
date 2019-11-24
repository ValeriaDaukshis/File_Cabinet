﻿using System;
using System.Collections.Generic;
using System.Linq;
using FileCabinetApp.CommandHandlers.Printer;
using FileCabinetApp.ExceptionClasses;
using FileCabinetApp.Memoization;
using FileCabinetApp.Records;
using FileCabinetApp.Service;

namespace FileCabinetApp.CommandHandlers.ServiceCommandHandlers
{
    /// <summary>
    /// SelectCommandHandler.
    /// </summary>
    public class SelectCommandHandler : ServiceCommandHandlerBase
    {
        private readonly ITablePrinter printer;
        private readonly IExpressionExtensions expressionExtensions;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectCommandHandler"/> class.
        /// DeleteCommandHandler constructor.
        /// </summary>
        /// <param name="cabinetService">fileCabinetService.</param>
        /// <param name="printer">The printer.</param>
        /// <param name="expressionExtensions">expressionExtensions.</param>
        public SelectCommandHandler(IFileCabinetService cabinetService, IExpressionExtensions expressionExtensions, ITablePrinter printer)
            : base(cabinetService)
        {
            this.printer = printer;
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

            if (commandRequest.Command == "select")
            {
                this.Select(commandRequest.Parameters);
            }
            else
            {
                this.NextHandler.Handle(commandRequest);
            }
        }

        private static void CheckConditionFieldsInput(string[] conditionFields)
        {
            if (conditionFields.Length > 2 && (!conditionFields.Contains("and") && !conditionFields.Contains("or")))
            {
                throw new ArgumentException($"{nameof(conditionFields)} parameters after 'where' don't have separator 'and(or)'. Use 'help' or 'syntax'", nameof(conditionFields));
            }
        }

        private static void CheckUpdateFieldsInput(string[] updatedFields)
        {
            if (updatedFields.Length == 0)
            {
                throw new ArgumentException($"{nameof(updatedFields)} Not enough parameters after command 'select'. Use 'help' or 'syntax'", nameof(updatedFields));
            }
        }

        private static (bool, IEnumerable<FileCabinetRecord>) SearchDataInCache(string[] conditionFields)
        {
            if (Cache.Contains(conditionFields))
            {
                return (true, Cache.GetValueByKey(conditionFields));
            }

            return (false, null);
        }

        private static void PutDataInCache(string[] conditionFields, IEnumerable<FileCabinetRecord> records)
        {
            Cache.Add(new CachingKey(conditionFields), records);
        }

        // select id, firstname, lastname where firstname = 'John' and lastname = 'Doe'
        private void Select(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                this.printer.Print(this.CabinetService.GetRecords(), FieldsCaseDictionary.Values.ToArray());
                return;
            }

            char[] separators = { '=', ',', ' ' };
            string[] inputs = parameters.Split("where", StringSplitOptions.RemoveEmptyEntries);
            string[] printedFields = inputs[0].Split(separators, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                CheckUpdateFieldsInput(printedFields);
                this.CommandHandlersExpressions.ChangeFieldCase(printedFields);
                if (inputs.Length == 1)
                {
                    this.printer.Print(this.CabinetService.GetRecords(), printedFields);
                    return;
                }

                var conditionFields = inputs[1].Split(separators, StringSplitOptions.RemoveEmptyEntries);

                (bool founded, IEnumerable<FileCabinetRecord> record) = SearchDataInCache(conditionFields);
                if (founded)
                {
                    this.printer.Print(record, printedFields);
                    return;
                }

                CheckConditionFieldsInput(conditionFields);
                string conditionSeparator = CommandHandlersExpressions.FindConditionSeparator(conditionFields);
                Dictionary<string, string> conditions = this.CommandHandlersExpressions.CreateDictionaryOfFields(conditionFields, conditionSeparator);

                // finds records that satisfy the condition
                var records = this.expressionExtensions.FindSuitableRecords(conditions.Values.ToArray(), conditions.Keys.ToArray(), conditionSeparator, typeof(FileCabinetRecord)).ToArray();

                this.printer.Print(records, printedFields);
                PutDataInCache(conditionFields, records);
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