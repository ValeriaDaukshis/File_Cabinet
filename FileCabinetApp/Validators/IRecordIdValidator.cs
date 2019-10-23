namespace FileCabinetApp.Validators
{
    /// <summary>
    /// IRecordIdValidator.
    /// </summary>
    public interface IRecordIdValidator
    {
        /// <summary>
        /// Tries the get record identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>true if record is exists.</returns>
        bool TryGetRecordId(int id);
    }
}