﻿using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
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
        protected static readonly CultureInfo Culture = CultureInfo.InvariantCulture;
        protected static readonly CultureInfo DateTimeCulture = new CultureInfo("en-US");
        protected static FileCabinetServiceSnapshot snapshot;
        protected ICommandHandler nextHandler;

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
        /// Gets or sets the record identifier validator.
        /// </summary>
        /// <value>
        /// The record identifier validator.
        /// </value>
        public static IRecordIdValidator RecordIdValidator { get;  set; }

        /// <summary>
        /// Sets the next.
        /// </summary>
        /// <param name="commandHandler">The command handler.</param>
        public void SetNext(ICommandHandler commandHandler)
        {
            nextHandler = commandHandler;
        }

        /// <summary>
        /// Handles the specified command request.
        /// </summary>
        /// <param name="commandRequest">The command request.</param>
        public abstract void Handle(AppCommandRequest commandRequest);

        protected static void PrintInputFields(out string firstName, out string lastName, out char gender, out DateTime dateOfBirth, out decimal credit, out short duration)
        {
            Console.Write("First name: ");
            firstName = ReadInput(stringConverter, firstNameValidator);
            Console.Write("Last name: ");
            lastName = ReadInput(stringConverter, lastNameValidator);
            Console.Write("Gender(M/F): ");
            gender = ReadInput(charConverter, genderValidator);
            Console.Write("Date of birth(mm/dd/yyyy): ");
            dateOfBirth = ReadInput(dateTimeConverter, dateOfBirthValidator);
            Console.Write("Credit sum(bel rub): ");
            credit = ReadInput(decimalConverter, creditSumValidator);
            Console.Write("Credit duration(month): ");
            duration = ReadInput(shortConverter, durationValidator);
        }

//        protected static void PrintRecords(ReadOnlyCollection<FileCabinetRecord> records)
//        {
//            foreach (var rec in records)
//            {
//                Console.WriteLine($"Id: {rec.Id}");
//                Console.WriteLine($"\tFirst name: {rec.FirstName}");
//                Console.WriteLine($"\tLast name: {rec.LastName}");
//                Console.WriteLine($"\tGender: {rec.Gender}");
//                Console.WriteLine($"\tDate of birth: {rec.DateOfBirth:yyyy-MMMM-dd}");
//                Console.WriteLine($"\tCredit sum: {rec.CreditSum}");
//                Console.WriteLine($"\tCredit duration: {rec.Duration}");
//            }
//        }

        protected static bool ImportExportParametersSpliter(string parameters, out string fileFormat, out string path)
        {
            fileFormat = string.Empty;
            path = string.Empty;
            if (string.IsNullOrEmpty(parameters))
            {
                Console.WriteLine("No parameters after command 'export'");
                return false;
            }

            string[] inputParameters = parameters.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

            if (inputParameters.Length < 2)
            {
                Console.WriteLine("Not enough parameters after command 'find'");
                return false;
            }

            fileFormat = inputParameters[0].ToLower(Culture);
            path = inputParameters[1];
            return true;
        }

        private static Func<string, Tuple<bool, string, string>> stringConverter = input =>
        {
            bool success = !string.IsNullOrEmpty(input) && input.Trim().Length != 0;
            return new Tuple<bool, string, string>(success, input, input.Trim());
        };

        private static Func<string, Tuple<bool, string, char>> charConverter = input =>
        {
            var result = default(char);
            bool success = char.TryParse(input?.Trim().ToUpper(Culture), out result);
            return new Tuple<bool, string, char>(success, input, result);
        };

        private static Func<string, Tuple<bool, string, DateTime>> dateTimeConverter = input =>
        {
            var result = default(DateTime);
            bool success = !string.IsNullOrEmpty(input) && DateTime.TryParse(input, DateTimeCulture, DateTimeStyles.None, out result);
            return new Tuple<bool, string, DateTime>(success, input, result);
        };

        private static Func<string, Tuple<bool, string, decimal>> decimalConverter = input =>
        {
            var result = default(decimal);
            bool success = !string.IsNullOrEmpty(input) && decimal.TryParse(input, out result);
            return new Tuple<bool, string, decimal>(success, input, result);
        };

        private static Func<string, Tuple<bool, string, short>> shortConverter = input =>
        {
            var result = default(short);
            bool success = !string.IsNullOrEmpty(input) && short.TryParse(input, out result);
            return new Tuple<bool, string, short>(success, input, result);
        };

        private static Func<string, Tuple<bool, string>> firstNameValidator = input =>
        {
            try
            {
                RecordValidator.ValidateFirstName(input);
            }
            catch (ArgumentNullException ex)
            {
                return new Tuple<bool, string>(false, ex.Message);
            }
            catch (ArgumentException ex)
            {
                return new Tuple<bool, string>(false, ex.Message);
            }

            return new Tuple<bool, string>(true, input);
        };

        private static Func<string, Tuple<bool, string>> lastNameValidator = input =>
        {
            try
            {
                RecordValidator.ValidateLastName(input);
            }
            catch (ArgumentNullException ex)
            {
                return new Tuple<bool, string>(false, ex.Message);
            }
            catch (ArgumentException ex)
            {
                return new Tuple<bool, string>(false, ex.Message);
            }

            return new Tuple<bool, string>(true, input);
        };

        private static Func<char, Tuple<bool, string>> genderValidator = input =>
        {
            try
            {
                RecordValidator.ValidateGender(input);
            }
            catch (ArgumentException ex)
            {
                return new Tuple<bool, string>(false, ex.Message);
            }

            return new Tuple<bool, string>(true, $"{input}");
        };

        private static Func<decimal, Tuple<bool, string>> creditSumValidator = input =>
        {
            try
            {
                RecordValidator.ValidateCreditSum(input);
            }
            catch (ArgumentException ex)
            {
                return new Tuple<bool, string>(false, ex.Message);
            }

            return new Tuple<bool, string>(true, $"{input}");
        };

        private static Func<DateTime, Tuple<bool, string>> dateOfBirthValidator = input =>
        {
            try
            {
                RecordValidator.ValidateDateOfBirth(input);
            }
            catch (ArgumentException ex)
            {
                return new Tuple<bool, string>(false, ex.Message);
            }

            return new Tuple<bool, string>(true, $"{input:MM/dd/yyyy}");
        };

        private static Func<short, Tuple<bool, string>> durationValidator = input =>
        {
            try
            {
                RecordValidator.ValidateDuration(input);
            }
            catch (ArgumentException ex)
            {
                return new Tuple<bool, string>(false, ex.Message);
            }

            return new Tuple<bool, string>(true, $"{input}");
        };

        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                T value;

                var input = Console.ReadLine();
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }
    }
}