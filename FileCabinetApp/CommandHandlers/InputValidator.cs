using System;
using System.Linq;
using FileCabinetApp.Converters;
using FileCabinetApp.Records;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// InputReader.
    /// </summary>
    public class InputReader
    {
        private static readonly Func<string, Tuple<bool, string>> FirstNameValidator = input =>
        {
            if (string.IsNullOrEmpty(input))
            {
                return new Tuple<bool, string>(false, "First name is empty");
            }

            if (input.Length < Program.ValidatorParams.FirstName.Min || input.Length > Program.ValidatorParams.FirstName.Max)
            {
                return new Tuple<bool, string>(false, $"First name length is upper than {Program.ValidatorParams.FirstName.Max} or under than {Program.ValidatorParams.FirstName.Min} symbols");
            }

            return new Tuple<bool, string>(true, input);
        };

        private static readonly Func<string, Tuple<bool, string>> LastNameValidator = input =>
        {
            if (string.IsNullOrEmpty(input))
            {
                return new Tuple<bool, string>(false, "Last name is empty");
            }

            if (input.Length < Program.ValidatorParams.LastName.Min || input.Length > Program.ValidatorParams.LastName.Max)
            {
                return new Tuple<bool, string>(false, $"Last name length is upper than {Program.ValidatorParams.LastName.Max} or under than {Program.ValidatorParams.LastName.Min} symbols");
            }

            return new Tuple<bool, string>(true, input);
        };

        private static readonly Func<char, Tuple<bool, string>> GenderValidator = gender =>
        {
            if (gender != 'M' && gender != 'F')
            {
                return new Tuple<bool, string>(false, "Indefinite gender");
            }

            return new Tuple<bool, string>(true, $"{gender}");
        };

        private static readonly Func<decimal, Tuple<bool, string>> CreditSumValidator = input =>
        {
            if (input > Program.ValidatorParams.CreditSum.Max || input < Program.ValidatorParams.CreditSum.Min)
            {
                return new Tuple<bool, string>(false, $"Credit sum is upper than {Program.ValidatorParams.CreditSum.Max} or under than {Program.ValidatorParams.CreditSum.Min} BYN");
            }

            return new Tuple<bool, string>(true, $"{input}");
        };

        private static readonly Func<DateTime, Tuple<bool, string>> DateOfBirthValidator = input =>
        {
            if (input > Program.ValidatorParams.DateOfBirth.To)
            {
                return new Tuple<bool, string>(false, $"Date of birth is upper than {Program.ValidatorParams.DateOfBirth.To:MM/dd/yyyy}");
            }

            if (input < Program.ValidatorParams.DateOfBirth.From)
            {
                return new Tuple<bool, string>(false, $"Date of birth is under than {Program.ValidatorParams.DateOfBirth.From:MM/dd/yyyy}");
            }

            return new Tuple<bool, string>(true, $"{input:MM/dd/yyyy}");
        };

        private static readonly Func<short, Tuple<bool, string>> DurationValidator = input =>
        {
            if (input > Program.ValidatorParams.Duration.To || input < Program.ValidatorParams.Duration.From)
            {
                return new Tuple<bool, string>(false, $"Duration is upper than {Program.ValidatorParams.Duration.To} or under than {Program.ValidatorParams.Duration.From}");
            }

            return new Tuple<bool, string>(true, $"{input}");
        };

        private static readonly Func<string[], string[], Tuple<bool, string>> FirstNameSearcher = (fields, values) =>
        {
            if (!fields.Contains("FirstName"))
            {
                return new Tuple<bool, string>(false, "Can not find FirstName member");
            }

            var index = fields.ToList().FindIndex(x => x == "FirstName");
            var item = values[index];

            return new Tuple<bool, string>(true, $"{item}");
        };

        private static readonly Func<string[], string[], Tuple<bool, string>> LastNameSearcher = (fields, values) =>
        {
            if (!fields.Contains("LastName"))
            {
                return new Tuple<bool, string>(false, "Can not find LastName member");
            }

            var index = fields.ToList().FindIndex(x => x == "LastName");
            var item = values[index];

            return new Tuple<bool, string>(true, $"{item}");
        };

        private static readonly Func<string[], string[], Tuple<bool, string>> GenderSearcher = (fields, values) =>
        {
            if (!fields.Contains("Gender"))
            {
                return new Tuple<bool, string>(false, "Can not find Gender member");
            }

            var index = fields.ToList().FindIndex(x => x == "Gender");
            var item = values[index];

            return new Tuple<bool, string>(true, $"{item}");
        };

        private static readonly Func<string[], string[], Tuple<bool, string>> CreditSumSearcher = (fields, values) =>
        {
            if (!fields.Contains("CreditSum"))
            {
                return new Tuple<bool, string>(false, "Can not find CreditSum member");
            }

            var index = fields.ToList().FindIndex(x => x == "CreditSum");
            var item = values[index];

            return new Tuple<bool, string>(true, $"{item}");
        };

        private static readonly Func<string[], string[], Tuple<bool, string>> DateOfBirthSearcher = (fields, values) =>
        {
            if (!fields.Contains("DateOfBirth"))
            {
                return new Tuple<bool, string>(false, "Can not find DateOfBirth member");
            }

            var index = fields.ToList().FindIndex(x => x == "DateOfBirth");
            var item = values[index];

            return new Tuple<bool, string>(true, $"{item}");
        };

        private static readonly Func<string[], string[], Tuple<bool, string>> DurationSearcher = (fields, values) =>
        {
            if (!fields.Contains("Duration"))
            {
                return new Tuple<bool, string>(false, "Can not find Duration member");
            }

            var index = fields.ToList().FindIndex(x => x == "Duration");
            var item = values[index];

            return new Tuple<bool, string>(true, $"{item}");
        };

        private readonly Converter converter;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputReader"/> class.
        /// </summary>
        /// <param name="converter">The converter.</param>
        public InputReader(Converter converter)
        {
            this.converter = converter;
        }

        /// <summary>
        /// PrintInputFields.
        /// </summary>
        /// <param name="modelWriter">Console Writer.</param>
        /// <returns>FileCabinetRecord.</returns>
        public FileCabinetRecord PrintInputFields(ModelWriters modelWriter)
        {
            if (modelWriter is null)
            {
                throw new ArgumentNullException(nameof(modelWriter), $"{nameof(modelWriter)} is null");
            }

            modelWriter.Writer.Invoke("First name: ");
            string firstName = ReadInput(this.converter.StringConverter, FirstNameValidator, modelWriter);
            modelWriter.Writer.Invoke("Last name: ");
            string lastName = ReadInput(this.converter.StringConverter, LastNameValidator, modelWriter);
            modelWriter.Writer.Invoke("Gender(M/F): ");
            char gender = ReadInput(this.converter.CharConverter, GenderValidator, modelWriter);
            modelWriter.Writer.Invoke("Date of birth(mm/dd/yyyy): ");
            DateTime dateOfBirth = ReadInput(this.converter.DateTimeConverter, DateOfBirthValidator, modelWriter);
            modelWriter.Writer.Invoke("Credit sum(bel rub): ");
            decimal credit = ReadInput(this.converter.DecimalConverter, CreditSumValidator, modelWriter);
            modelWriter.Writer.Invoke("Credit duration(month): ");
            short duration = ReadInput(this.converter.ShortConverter, DurationValidator, modelWriter);
            return new FileCabinetRecord(firstName, lastName, gender, dateOfBirth, credit, duration);
        }

        /// <summary>
        /// Prints the input fields.
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <param name="values">The values.</param>
        /// <returns>FileCabinetRecord.</returns>
        public FileCabinetRecord PrintInputFields(string[] fields, string[] values)
        {
            string firstName = ReadInput(fields, values, FirstNameSearcher, this.converter.StringConverter, FirstNameValidator);
            string lastName = ReadInput(fields, values, LastNameSearcher, this.converter.StringConverter, LastNameValidator);
            char gender = ReadInput(fields, values, GenderSearcher, this.converter.CharConverter, GenderValidator);
            DateTime dateOfBirth = ReadInput(fields, values, DateOfBirthSearcher, this.converter.DateTimeConverter, DateOfBirthValidator);
            decimal credit = ReadInput(fields, values, CreditSumSearcher, this.converter.DecimalConverter, CreditSumValidator);
            short duration = ReadInput(fields, values, DurationSearcher, this.converter.ShortConverter, DurationValidator);
            return new FileCabinetRecord(firstName, lastName, gender, dateOfBirth, credit, duration);
        }

        /// <summary>
        /// Checks the input fields.
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <param name="values">The values.</param>
        /// <param name="record">The record.</param>
        /// <returns>FileCabinetRecord.</returns>
        public FileCabinetRecord CheckInputFields(string[] fields, string[] values, FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record), $"{nameof(record)} is null");
            }

            string firstName = ReadInput(fields, values, record.FirstName, FirstNameSearcher, this.converter.StringConverter, FirstNameValidator);
            string lastName = ReadInput(fields, values, record.LastName, LastNameSearcher, this.converter.StringConverter, LastNameValidator);
            char gender = ReadInput(fields, values, record.Gender, GenderSearcher, this.converter.CharConverter, GenderValidator);
            DateTime dateOfBirth = ReadInput(fields, values, record.DateOfBirth, DateOfBirthSearcher, this.converter.DateTimeConverter, DateOfBirthValidator);
            decimal credit = ReadInput(fields, values, record.CreditSum, CreditSumSearcher, this.converter.DecimalConverter, CreditSumValidator);
            short duration = ReadInput(fields, values, record.Duration, DurationSearcher, this.converter.ShortConverter, DurationValidator);
            return new FileCabinetRecord(firstName, lastName, gender, dateOfBirth, credit, duration);
        }

        private static T ReadInput<T>(string[] fields, string[] values, Func<string[], string[], Tuple<bool, string>> finder, Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            if (finder is null)
            {
                throw new ArgumentNullException(nameof(finder), $"{nameof(finder)} is null");
            }

            T value;
            var inputResult = finder(fields, values);
            if (!inputResult.Item1)
            {
                throw new ArgumentException($"Conversion failed: {inputResult.Item2}. Please, correct your input.");
            }

            string input = inputResult.Item2;

            var conversionResult = converter(input);

            if (!conversionResult.Item1)
            {
                throw new ArgumentException($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
            }

            value = conversionResult.Item3;

            var validationResult = validator(value);
            if (!validationResult.Item1)
            {
                throw new ArgumentException($"Validation failed: {validationResult.Item2}. Please, correct your input.");
            }

            return value;
        }

        private static T ReadInput<T>(string[] fields, string[] values, T record, Func<string[], string[], Tuple<bool, string>> finder, Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            if (finder is null)
            {
                throw new ArgumentNullException(nameof(finder), $"{nameof(finder)} is null");
            }

            T value;
            var inputResult = finder(fields, values);
            if (!inputResult.Item1)
            {
                return record;
            }

            string input = inputResult.Item2;

            var conversionResult = converter(input);

            if (!conversionResult.Item1)
            {
                throw new ArgumentException($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
            }

            value = conversionResult.Item3;

            var validationResult = validator(value);
            if (!validationResult.Item1)
            {
                throw new ArgumentException($"Validation failed: {validationResult.Item2}. Please, correct your input.");
            }

            return value;
        }

        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator, ModelWriters modelWriter)
        {
            do
            {
                T value;

                var input = modelWriter.Reader.Invoke();
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    modelWriter.LineWriter.Invoke($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    modelWriter.LineWriter.Invoke($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }
    }
}