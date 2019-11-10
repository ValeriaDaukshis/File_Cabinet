using System;
using FileCabinetApp.Records;

namespace FileCabinetApp.Service
{
    public class MemoryIterator<FileCabinetRecord> : IRecordIterator<FileCabinetRecord>
    {
        private FileCabinetRecord[] collection;
        private int currentIndex;

        public MemoryIterator(FileCabinetRecord[] records)
        {
            this.collection = records;
            this.currentIndex = -1;
        }

        public bool MoveNext()
        {
            return ++this.currentIndex < this.collection.Length;
        }

        public void Reset()
        {
            this.currentIndex = -1;
        }

        public FileCabinetRecord Current()
        {
            if (this.currentIndex == -1 || this.currentIndex == this.collection.Length)
            {
                throw new InvalidOperationException();
            }

            return this.collection[this.currentIndex];
        }
    }
}