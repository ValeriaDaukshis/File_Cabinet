using System.Linq;
using FileCabinetApp.ExceptionClasses;
using FileCabinetApp.Service;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// RecordIdMemoryValidator.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Validators.IRecordIdValidator" />
    public class RecordIdMemoryValidator : IRecordIdValidator
    {
        private readonly IFileCabinetService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordIdMemoryValidator"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        public RecordIdMemoryValidator(IFileCabinetService service)
        {
            this.service = service;
        }

        /// <summary>
        /// Tries the get record identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>true if record with id is exists.</returns>
        public bool TryGetRecordId(int id)
        {
            var list = this.service.GetRecords().ToList();
            if (list.Find(x => x.Id == id) is null)
            {
                throw new FileRecordNotFoundException(id);
            }

            return true;
        }
    }
}