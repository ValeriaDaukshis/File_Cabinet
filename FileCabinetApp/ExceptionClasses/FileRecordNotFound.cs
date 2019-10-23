using System;

namespace FileCabinetApp.ExceptionClasses
{
    /// <summary>
    /// FileRecordNotFound.
    /// </summary>
    public class FileRecordNotFound : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileRecordNotFound"/> class.
        /// </summary>
        /// <param name="val">value.</param>
        public FileRecordNotFound(int val)
            : base()
        {
            this.Value = val;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileRecordNotFound"/> class.
        /// </summary>
        /// <param name="message">message.</param>
        public FileRecordNotFound(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileRecordNotFound"/> class.
        /// </summary>
        /// <param name="message">message.</param>
        /// <param name="innerException">inner Exception.</param>
        public FileRecordNotFound(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileRecordNotFound"/> class.
        /// </summary>
        public FileRecordNotFound()
        {
        }

        /// <summary>
        /// Gets value.
        /// </summary>
        /// <value>
        /// Value.
        /// </value>
        public int Value { get; }
    }
}