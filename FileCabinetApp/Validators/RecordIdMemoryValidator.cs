using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FileCabinetApp.ExceptionClasses;
using FileCabinetApp.Service;

namespace FileCabinetApp.Validators
{
    public class RecordIdMemoryValidator : IRecordIdValidator
    {
        private readonly IFileCabinetService service;

        public RecordIdMemoryValidator(IFileCabinetService service)
        {
            this.service = service;
        }

        public bool TryGetRecordId(int id)
        {
            var list = service.GetRecords().ToList();
            if (list.Find(x => x.Id == id) is null)
            {
                throw new FileRecordNotFound(id);
            }

            return true;
        }
    }
}