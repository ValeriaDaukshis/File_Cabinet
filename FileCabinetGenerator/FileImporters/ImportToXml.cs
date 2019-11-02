using System.IO;
using System.Xml.Serialization;
using FileCabinetApp;
using FileCabinetApp.Records;

namespace FileCabinetGenerator.FileImporters
{
    /// <summary>
    /// ImportToXml.
    /// </summary>
    public class ImportToXml : IFileCabinetSerializer
    {
        private readonly StreamWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportToXml"/> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public ImportToXml(StreamWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Serializes the specified records.
        /// </summary>
        /// <param name="records">The records.</param>
        public void Serialize(FileCabinetRecords records)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(FileCabinetRecords));

            using (writer)
            {
                formatter.Serialize(writer, records);
            }
        }
    }
}