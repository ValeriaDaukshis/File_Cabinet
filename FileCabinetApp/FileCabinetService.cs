using System;
using System.Collections.Generic;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth)
        {
            this.list.Add(new FileCabinetRecord { FirstName = firstName, LastName = lastName, DateOfBirth = dateOfBirth });
            return 0;
        }

        public static FileCabinetRecord[] GetRecords()
        {
            return Array.Empty<FileCabinetRecord>();
        }

        public int GetStat()
        {
            return this.list.Count;
        }
    }
}