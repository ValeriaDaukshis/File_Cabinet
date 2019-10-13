using System;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// FileCabinetDefaultService.
    /// </summary>
    /// <seealso cref="FileCabinetApp.FileCabinetService" />
    public class FileCabinetDefaultService : FileCabinetService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetDefaultService"/> class.
        /// </summary>
        public FileCabinetDefaultService()
            : base(new DefaultValidator())
        {
        }
    }
}