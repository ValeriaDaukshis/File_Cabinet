using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using FileCabinetApp.ExceptionClasses;
using FileCabinetApp.Records;
using FileCabinetApp.Service;

namespace FileCabinetApp.CommandHandlers.ServiceCommandHandlers
{
    /// <summary>
    /// UpdateCommandHandler.
    /// </summary>
    public class UpdateCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommandHandler"/> class.
        /// DeleteCommandHandler constructor.
        /// </summary>
        /// <param name="cabinetService">fileCabinetService.</param>
        public UpdateCommandHandler(IFileCabinetService cabinetService)
            : base(cabinetService)
        {
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
            if (conditionFields.Length < 2)
            {
                throw new ArgumentException($"{nameof(conditionFields)} Not enough parameters after condition command 'where'", nameof(conditionFields));
            }

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
            if (updatedFields.Length < 3)
            {
                throw new ArgumentException($"{nameof(updatedFields)} Not enough parameters after condition command 'update'", nameof(updatedFields));
            }

            if (updatedFields.Length > 2 && !updatedFields.Contains("set"))
            {
                throw new ArgumentException($"{nameof(updatedFields)} parameters after 'update' don't have separator 'set''", nameof(updatedFields));
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

        private void Update(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                Console.WriteLine("Write a record number");
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
                var records = this.FindSuitableRecords(conditions.Values.ToArray(), conditions.Keys.ToArray(), conditionSeparator).ToArray();

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