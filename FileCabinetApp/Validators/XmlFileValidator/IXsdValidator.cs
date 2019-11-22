namespace FileCabinetApp.Validators.XsdValidator
{
    /// <summary>
    /// IXsdValidator.
    /// </summary>
    public interface IXsdValidator
    {
        /// <summary>
        /// Validates the XML.
        /// </summary>
        /// <param name="validator">The validator.</param>
        /// <param name="fileName">Name of the file.</param>
        void ValidateXml(string validator, string fileName);
    }
}