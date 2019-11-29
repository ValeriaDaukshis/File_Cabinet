namespace FileCabinetApp.Validators.XmlFileValidator
{
    /// <summary>
    /// IXsdValidator.
    /// </summary>
    public interface IXmlValidator
    {
        /// <summary>
        /// Validates the XML.
        /// </summary>
        /// <param name="validator">The validator.</param>
        /// <param name="fileName">Name of the file.</param>
        void ValidateXml(string validator, string fileName);
    }
}