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
    /// DeleteCommandHandler.
    /// </summary>
    public class DeleteCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCommandHandler"/> class.
        /// DeleteCommandHandler constructor.
        /// </summary>
        /// <param name="cabinetService">fileCabinetService.</param>
        public DeleteCommandHandler(IFileCabinetService cabinetService)
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

            if (commandRequest.Command == "delete")
            {
                this.Delete(commandRequest.Parameters);
            }
            else
            {
                this.NextHandler.Handle(commandRequest);
            }
        }

        private static string CreateOutputText(int[] recordsId)
        {
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

        private void Delete(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                Console.WriteLine("Write a record number");
                return;
            }

            char[] separator = { ' ', '=' };
            string[] inputs = parameters.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            if (inputs.Length != 3)
            {
                Console.WriteLine("Not enough parameters after command 'delete'");
                return;
            }

            string fieldName = inputs[1];
            string parameter = inputs[2];
            if (parameter[0] == '\'' || parameter[0] == '"')
            {
                parameter = parameter.Substring(1, parameter.Length - 2);
            }

            try
            {
                List<int> recordsId = new List<int>();
                var records = this.DeleteRecord(parameter, fieldName).ToArray();
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

        private IEnumerable<FileCabinetRecord> DeleteRecord(string parameter, string fieldName)
        {
            Type type = typeof(FileCabinetRecord);

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

            Func<FileCabinetRecord, bool> delegateForWhere = whereExpression.Compile();
            var records = from n in this.CabinetService.GetRecords()
                where delegateForWhere.Invoke(n)
                select n;

            return records;
        }
    }
}