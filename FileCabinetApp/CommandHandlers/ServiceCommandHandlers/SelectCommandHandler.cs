using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
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
        private ITablePrinter printer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectCommandHandler"/> class.
        /// DeleteCommandHandler constructor.
        /// </summary>
        /// <param name="cabinetService">fileCabinetService.</param>
        /// <param name="printer">The printer.</param>
        public SelectCommandHandler(IFileCabinetService cabinetService, ITablePrinter printer)
            : base(cabinetService)
        {
            this.printer = printer;
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

        private static void CorrectValuesInput(string[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i][0] == '\'' || values[i][0] == '"')
                {
                    values[i] = values[i].Substring(1, values[i].Length - 2);
                }
            }
        }

        private static Expression<Func<FileCabinetRecord, bool>> MakeExpressionTree(string parameter, string fieldName, Type type)
        {
            ParameterExpression parameterValue = Expression.Parameter(type, "field");
            PropertyInfo propertyInfo = type.GetProperty(fieldName);

            if (propertyInfo is null)
            {
                throw new ArgumentNullException(nameof(fieldName), $"{nameof(propertyInfo)} is null");
            }

            MemberExpression property = Expression.MakeMemberAccess(parameterValue, propertyInfo);

            TypeConverter typeConverter = TypeDescriptor.GetConverter(propertyInfo.PropertyType);
            var convertedParameter = typeConverter.ConvertFrom(parameter);
            ConstantExpression value = Expression.Constant(convertedParameter, propertyInfo.PropertyType);

            BinaryExpression greaterThanConstantValue = Expression.Equal(property, value);

            Expression<Func<FileCabinetRecord, bool>> whereExpression =
                Expression.Lambda<Func<FileCabinetRecord, bool>>(greaterThanConstantValue, parameterValue);

            return whereExpression;
        }

        private static void CheckInputConditionFields(string[] conditionFields)
        {
            if (conditionFields.Length > 2 && (!conditionFields.Contains("and") && !conditionFields.Contains("or")))
            {
                throw new ArgumentException($"{nameof(conditionFields)} parameters after 'where' don't have separator 'and(or)''", nameof(conditionFields));
            }
        }

        private static string CheckConditionSeparator(string[] conditionFields)
        {
            if (conditionFields.Contains("and") && conditionFields.Contains("or"))
            {
                throw new ArgumentException($"{nameof(conditionFields)} request can not contains separator 'and', 'or' together", nameof(conditionFields));
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
            if (updatedFields.Length == 0)
            {
                throw new ArgumentException($"{nameof(updatedFields)} Not enough parameters after command 'select'", nameof(updatedFields));
            }
        }

        private static void UpdatePrintedFields(string[] printedFields)
        {
            for (int i = 0; i < printedFields.Length; i++)
            {
                var key = printedFields[i].ToLower(Culture);

                if (!FieldsCaseDictionary.ContainsKey(key))
                {
                    throw new ArgumentException($"No field with name {nameof(printedFields)}", nameof(printedFields));
                }

                printedFields[i] = FieldsCaseDictionary[key];
            }
        }

        private static Dictionary<string, string> CreateFieldsDictionary(string[] values, string separator)
        {
            Dictionary<string, string> updates = new Dictionary<string, string>();
            CorrectValuesInput(values);

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

        private static (bool, IEnumerable<FileCabinetRecord>) SearchDataInCache(string[] conditionFields)
        {
            if (Cache.Contains(conditionFields))
            {
                return (true, Cache.GetValueByKey(conditionFields));
            }

            return (false, null);
        }

        private static void PushDataInCache(string[] conditionFields, IEnumerable<FileCabinetRecord> records)
        {
            Cache.Add(new DataCachingKey(conditionFields), records);
        }

        // select id, firstname, lastname where firstname = 'John' and lastname = 'Doe'
        private void Select(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                Console.WriteLine("Write information");
                return;
            }

            char[] separators = { '=', ',', ' ' };
            string[] inputs = parameters.Split("where", StringSplitOptions.RemoveEmptyEntries);
            string[] printedFields = inputs[0].Split(separators, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                CheckInputUpdateFields(printedFields);
                UpdatePrintedFields(printedFields);
                string[] conditionFields;
                if (inputs.Length == 1)
                {
                    this.printer.Print(this.CabinetService.GetRecords(), printedFields);
                    return;
                }

                conditionFields = inputs[1].Split(separators, StringSplitOptions.RemoveEmptyEntries);

                (bool founded, IEnumerable<FileCabinetRecord> record) = SearchDataInCache(conditionFields);
                if (founded)
                {
                    this.printer.Print(record, printedFields);
                    return;
                }

                CheckInputConditionFields(conditionFields);
                string conditionSeparator = CheckConditionSeparator(conditionFields);
                Dictionary<string, string> conditions = CreateFieldsDictionary(conditionFields, conditionSeparator);

                // finds records that satisfy the condition
                var records = this.FindSuitableRecords(conditions.Values.ToArray(), conditions.Keys.ToArray(), conditionSeparator).ToArray();

                this.printer.Print(records, printedFields);
                PushDataInCache(conditionFields, records);
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

        private IEnumerable<FileCabinetRecord> FindSuitableRecords(string[] parameter, string[] fieldName, string conditionSeparator)
        {
            Type type = typeof(FileCabinetRecord);

            var expressionTree = MakeExpressionTree(parameter[0], fieldName[0], type);
            Expression<Func<FileCabinetRecord, bool>> delegateForSearch = expressionTree;

            for (int i = 1; i < parameter.Length; i++)
            {
                var expression = MakeExpressionTree(parameter[i], fieldName[i], type);
                var invokedExpr = Expression.Invoke(expression, expressionTree.Parameters.Cast<Expression>());

                if (conditionSeparator == "and")
                {
                    delegateForSearch = Expression.Lambda<Func<FileCabinetRecord, bool>>(
                        Expression.AndAlso(expressionTree.Body, invokedExpr), expressionTree.Parameters);
                }
                else if (conditionSeparator == "or")
                {
                    delegateForSearch = Expression.Lambda<Func<FileCabinetRecord, bool>>(
                        Expression.OrElse(expressionTree.Body, invokedExpr), expressionTree.Parameters);
                }
                else
                {
                    throw new ArgumentException($"Incorrect condition separator {nameof(conditionSeparator)}. Use 'and' || 'or'", nameof(conditionSeparator));
                }
            }

            Func<FileCabinetRecord, bool> delegateForWhere = delegateForSearch.Compile();

            var records = from n in this.CabinetService.GetRecords()
                where delegateForWhere.Invoke(n)
                select n;

            return records;
        }
    }
}