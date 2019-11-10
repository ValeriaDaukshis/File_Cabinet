using System;
using System.Collections;
using System.Collections.Generic;
using FileCabinetApp.Records;

namespace FileCabinetApp.Service
{
    public class FilesystemIterator<FileCabinetRecord> : IEnumerator<FileCabinetRecord>
    {
        private FileCabinetRecord[] collection;
        private int currentIndex;
        private object current;
        private FileCabinetRecord current1;
        private bool disposed = false;

        public FilesystemIterator(FileCabinetRecord[] records)
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

        FileCabinetRecord IEnumerator<FileCabinetRecord>.Current => current1;

        object IEnumerator.Current => current;

        public FileCabinetRecord Current
        {
            get
            {
                if (this.currentIndex == -1 || this.currentIndex == this.collection.Length)
                {
                    throw new InvalidOperationException();
                }

                return this.collection[this.currentIndex];
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
            }

            this.disposed = true;
        }
    }
}