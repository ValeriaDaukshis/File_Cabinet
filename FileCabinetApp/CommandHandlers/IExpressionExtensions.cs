using System;
using System.Collections.Generic;
using FileCabinetApp.Records;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// IExpressionExtensions.
    /// </summary>
    public interface IExpressionExtensions
    {
        /// <summary>
        /// Finds the suitable records.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="conditionSeparator">The condition separator.</param>
        /// <param name="classType">Type of the class.</param>
        /// <returns>records that suits the condition.</returns>
        IEnumerable<FileCabinetRecord> FindSuitableRecords(string[] parameter, string[] fieldName, string conditionSeparator, Type classType);

        /// <summary>
        /// Finds the suitable records.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="classType">Type of the class.</param>
        /// <returns>records that suits the condition.</returns>
        IEnumerable<FileCabinetRecord> FindSuitableRecords(string parameter, string fieldName, Type classType);
    }
}