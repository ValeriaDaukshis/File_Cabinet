using FileCabinetRecords = FileCabinetGenerator.Records.FileCabinetRecords;

namespace FileCabinetGenerator.FileImporters
{
    /// <summary>
    /// IFileCabinetSerializer.
    /// </summary>
    interface IFileCabinetSerializer
    {
        /// <summary>
        /// Serializes the specified record.
        /// </summary>
        /// <param name="record">The record.</param>
        void Serialize(FileCabinetRecords record);
    }
}
