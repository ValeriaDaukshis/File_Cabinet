using System.IO;
using FileCabinetApp.ExceptionClasses;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// RecordIdFilesystemValidator.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Validators.IRecordIdValidator" />
    public class RecordIdFilesystemValidator : IRecordIdValidator
    {
        private const long RecordSize = (sizeof(short) * 2) + (120 * 2) + sizeof(char) + (sizeof(int) * 4) + sizeof(decimal);

        private readonly FileStream fileStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordIdFilesystemValidator"/> class.
        /// </summary>
        /// <param name="fileStream">The file stream.</param>
        public RecordIdFilesystemValidator(FileStream fileStream)
        {
            this.fileStream = fileStream;
        }

        /// <summary>
        /// Tries the get record identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>true if record with id is exists.</returns>
        public bool TryGetRecordId(int id)
        {
            BinaryReader reader = new BinaryReader(this.fileStream);
            if (!this.TryGetFileRecordPosition(reader, id))
            {
                throw new FileRecordNotFoundException(id);
            }

            return true;
        }

        private bool TryGetFileRecordPosition(BinaryReader reader, int recordId)
        {
            reader.BaseStream.Position = 0;
            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                reader.BaseStream.Position += sizeof(short);
                var id = reader.ReadInt32();

                if (id == recordId)
                {
                    return true;
                }

                reader.BaseStream.Position += RecordSize - sizeof(int) - sizeof(short);
            }

            return false;
        }
    }
}