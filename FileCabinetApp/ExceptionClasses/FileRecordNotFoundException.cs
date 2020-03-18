using System;

namespace FileCabinetApp.ExceptionClasses
{
    /// <summary>
    ///     FileRecordNotFound.
    /// </summary>
    public class FileRecordNotFoundException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FileRecordNotFoundException" /> class.
        /// </summary>
        /// <param name="val">value.</param>
        public FileRecordNotFoundException(int val)
        {
            this.Value = val;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FileRecordNotFoundException" /> class.
        /// </summary>
        /// <param name="message">message.</param>
        public FileRecordNotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FileRecordNotFoundException" /> class.
        /// </summary>
        /// <param name="message">message.</param>
        /// <param name="innerException">inner Exception.</param>
        public FileRecordNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FileRecordNotFoundException" /> class.
        /// </summary>
        public FileRecordNotFoundException()
        {
        }

        /// <summary>
        ///     Gets value.
        /// </summary>
        /// <value>
        ///     Value.
        /// </value>
        public int Value { get; }
    }
}