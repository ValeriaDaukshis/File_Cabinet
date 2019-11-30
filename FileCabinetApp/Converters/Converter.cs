using System;
using System.Globalization;

namespace FileCabinetApp.Converters
{
    /// <summary>
    ///     Converter.
    /// </summary>
    public class Converter
    {
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

        private static readonly CultureInfo DateTimeCulture = new CultureInfo("en-US");

        /// <summary>
        ///     Gets the character converter.
        /// </summary>
        /// <value>
        ///     The character converter.
        /// </value>
        public Func<string, Tuple<bool, string, char>> CharConverter { get; } = input =>
        {
            var result = '\x0000';
            var success = char.TryParse(input?.Trim().ToUpper(Culture), out result);
            return new Tuple<bool, string, char>(success, input, result);
        };

        /// <summary>
        ///     Gets the date time converter.
        /// </summary>
        /// <value>
        ///     The date time converter.
        /// </value>
        public Func<string, Tuple<bool, string, DateTime>> DateTimeConverter { get; } = input =>
        {
            var result = DateTime.Now;
            var success = !string.IsNullOrEmpty(input) && DateTime.TryParse(input, DateTimeCulture, DateTimeStyles.None, out result);
            return new Tuple<bool, string, DateTime>(success, input, result);
        };

        /// <summary>
        ///     Gets the decimal converter.
        /// </summary>
        /// <value>
        ///     The decimal converter.
        /// </value>
        public Func<string, Tuple<bool, string, decimal>> DecimalConverter { get; } = input =>
        {
            var result = 0.0m;
            var success = !string.IsNullOrEmpty(input) && decimal.TryParse(input, NumberStyles.AllowDecimalPoint, Culture, out result);
            return new Tuple<bool, string, decimal>(success, input, result);
        };

        /// <summary>
        ///     Gets the int converter.
        /// </summary>
        /// <value>
        ///     The int converter.
        /// </value>
        public Func<string, Tuple<bool, string, int>> IntConverter { get; } = input =>
        {
            var result = 0;
            var success = !string.IsNullOrEmpty(input) && int.TryParse(input, out result);
            return new Tuple<bool, string, int>(success, input, result);
        };

        /// <summary>
        ///     Gets the short converter.
        /// </summary>
        /// <value>
        ///     The short converter.
        /// </value>
        public Func<string, Tuple<bool, string, short>> ShortConverter { get; } = input =>
        {
            short result = 0;
            var success = !string.IsNullOrEmpty(input) && short.TryParse(input, out result);
            return new Tuple<bool, string, short>(success, input, result);
        };

        /// <summary>
        ///     Gets the string converter.
        /// </summary>
        /// <value>
        ///     The string converter.
        /// </value>
        public Func<string, Tuple<bool, string, string>> StringConverter { get; } = input =>
        {
            var success = !string.IsNullOrEmpty(input) && input.Trim().Length != 0;
            return new Tuple<bool, string, string>(success, input, input.Trim());
        };
    }
}