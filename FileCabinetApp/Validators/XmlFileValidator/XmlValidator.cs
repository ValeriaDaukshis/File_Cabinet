using System;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace FileCabinetApp.Validators.XmlFileValidator
{
    /// <summary>
    /// XsdValidator.
    /// </summary>
    /// <seealso cref="IXmlValidator" />
    public class XmlValidator : IXmlValidator
    {
        /// <summary>
        /// Validates the XML.
        /// </summary>
        /// <param name="validator">The validator.</param>
        /// <param name="fileName">Name of the file.</param>
        public void ValidateXml(string validator, string fileName)
        {
            XmlSchemaSet schema = new XmlSchemaSet();
            schema.Add(string.Empty, validator);

            using (XmlReader rd = XmlReader.Create(fileName))
            {
                XDocument doc = XDocument.Load(rd);
                doc.Validate(schema, ValidationEventHandler);
            }
        }

        private static void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            throw new ArgumentException($"{e.Message}\n {sender}\n");
        }
    }
}