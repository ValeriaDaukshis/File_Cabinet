using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FileCabinetApp.Records;
using FileCabinetApp.Service;

namespace FileCabinetApp.CommandHandlers.Extensions
{
    /// <summary>
    /// ExpressionExtensions.
    /// </summary>
    /// <seealso cref="IExpressionExtensions" />
    public class ExpressionExtensions : IExpressionExtensions
    {
        private readonly IFileCabinetService fileCabinetService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionExtensions"/> class.
        /// </summary>
        /// <param name="fileCabinetService">The file cabinet service.</param>
        public ExpressionExtensions(IFileCabinetService fileCabinetService)
        {
            this.fileCabinetService = fileCabinetService;
        }

        /// <summary>
        /// Finds the suitable records.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="conditionSeparator">The condition separator.</param>
        /// <param name="type">The type.</param>
        /// <returns>records that suits the condition.</returns>
        /// <exception cref="ArgumentException">Incorrect condition separator {nameof(conditionSeparator)}. Use 'and' || 'or' - conditionSeparator.</exception>
        public IEnumerable<FileCabinetRecord> FindSuitableRecords(string[] parameter, string[] fieldName, string conditionSeparator, Type type)
        {
            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter), $"{nameof(parameter)} is null");
            }

            if (fieldName is null)
            {
                throw new ArgumentNullException(nameof(fieldName), $"{nameof(fieldName)} is null");
            }

            if (conditionSeparator is null)
            {
                throw new ArgumentNullException(nameof(conditionSeparator), $"{nameof(conditionSeparator)} is null");
            }

            if (type is null)
            {
                throw new ArgumentNullException(nameof(type), $"{nameof(type)} is null");
            }

            var expressionTree = this.MakeExpressionTree(parameter[0], fieldName[0], type);
            Expression<Func<FileCabinetRecord, bool>> delegateForSearch = expressionTree;

            for (int i = 1; i < parameter.Length; i++)
            {
                var expression = this.MakeExpressionTree(parameter[i], fieldName[i], type);
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

            var records = from n in this.fileCabinetService.GetRecords()
                where delegateForWhere.Invoke(n)
                select n;

            return records;
        }

        /// <summary>
        /// Finds the suitable records.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="type">The type.</param>
        /// <returns>records that suits the condition.</returns>
        public IEnumerable<FileCabinetRecord> FindSuitableRecords(string parameter, string fieldName, Type type)
        {
            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter), $"{nameof(parameter)} is null");
            }

            if (fieldName is null)
            {
                throw new ArgumentNullException(nameof(fieldName), $"{nameof(fieldName)} is null");
            }

            if (type is null)
            {
                throw new ArgumentNullException(nameof(type), $"{nameof(type)} is null");
            }

            var expressionTree = this.MakeExpressionTree(parameter, fieldName, type);
            Func<FileCabinetRecord, bool> delegateForWhere = expressionTree.Compile();

            var records = from n in this.fileCabinetService.GetRecords()
                where delegateForWhere.Invoke(n)
                select n;

            return records;
        }

        private Expression<Func<FileCabinetRecord, bool>> MakeExpressionTree(string parameter, string fieldName, Type classType)
        {
            ParameterExpression parameterValue = Expression.Parameter(classType, "field");
            PropertyInfo propertyInfo = classType.GetProperty(fieldName);

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
    }
}