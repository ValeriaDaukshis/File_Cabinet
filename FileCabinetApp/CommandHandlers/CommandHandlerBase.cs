using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using FileCabinetApp.CommandHandlers.Extensions;
using FileCabinetApp.Converters;
using FileCabinetApp.Memoization;
using FileCabinetApp.Records;
using FileCabinetApp.Service;
using FileCabinetApp.Validators;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// CommandHandlerBase.
    /// </summary>
    /// <seealso cref="FileCabinetApp.CommandHandlers.ICommandHandler" />
    public abstract class CommandHandlerBase : ICommandHandler
    {
        /// <summary>
        /// Gets the converter.
        /// </summary>
        /// <value>
        /// The converter.
        /// </value>
        protected static readonly Converter Converter = new Converter();

        /// <summary>
        /// culture info.
        /// </summary>
        protected static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

        /// <summary>
        /// The fields case dictionary.
        /// </summary>
        protected static readonly Dictionary<string, string> FieldsCaseDictionary = new Dictionary<string, string>
        {
            { "id", "Id" },
            { "firstname", "FirstName" },
            { "lastname", "LastName" },
            { "gender", "Gender" },
            { "dateofbirth", "DateOfBirth" },
            { "creditsum", "CreditSum" },
            { "duration", "Duration" },
        };

        /// <summary>
        /// Gets or sets the record validator.
        /// </summary>
        /// <value>
        /// The record validator.
        /// </value>
        public static IRecordValidator RecordValidator { get;  set; }

        /// <summary>
        /// Gets or sets the service storage file stream.
        /// </summary>
        /// <value>
        /// The service storage file stream.
        /// </value>
        public static FileStream ServiceStorageFileStream { get;  set; }

        /// <summary>
        /// Gets cache.
        /// </summary>
        /// <value>
        /// Cache.
        /// </value>
        protected static DataCaching Cache { get; } = new DataCaching();

        /// <summary>
        /// Gets or sets iCommandHandler.
        /// </summary>
        /// <value>
        /// ICommandHandler.
        /// </value>
        protected ICommandHandler NextHandler { get; set; }

        /// <summary>
        /// Gets the input reader.
        /// </summary>
        /// <value>
        /// The input reader.
        /// </value>
        protected InputValidator InputValidator { get; } = new InputValidator(Converter);

        /// <summary>
        /// Gets the command handlers expressions.
        /// </summary>
        /// <value>
        /// The command handlers expressions.
        /// </value>
        protected CommandHandlersExtensions CommandHandlersExtensions { get; } = new CommandHandlersExtensions(FieldsCaseDictionary);

        /// <summary>
        /// Sets the next.
        /// </summary>
        /// <param name="commandHandler">The command handler.</param>
        /// <returns>ICommandHandler.</returns>
        public ICommandHandler SetNext(ICommandHandler commandHandler)
        {
            this.NextHandler = commandHandler;
            return this.NextHandler;
        }

        /// <summary>
        /// Handles the specified command request.
        /// </summary>
        /// <param name="commandRequest">The command request.</param>
        public abstract void Handle(AppCommandRequest commandRequest);
    }
}