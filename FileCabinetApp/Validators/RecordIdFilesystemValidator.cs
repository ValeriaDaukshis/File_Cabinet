using System.IO;
using FileCabinetApp.ExceptionClasses;

namespace FileCabinetApp.Validators
{
    public class RecordIdFilesystemValidator : IRecordIdValidator
    {
        private const long RecordSize = (sizeof(short) * 2) + (120 * 2) + sizeof(char) + (sizeof(int) * 4) + sizeof(decimal);

        private readonly FileStream fileStream;

        public RecordIdFilesystemValidator(FileStream fileStream)
        {
            this.fileStream = fileStream;
        }

        public bool TryGetRecordId(int id)
        {
            BinaryReader reader = new BinaryReader(fileStream);
            if (!this.TryGetFileRecordPosition(reader, id))
            {
                throw new FileRecordNotFound(id);
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