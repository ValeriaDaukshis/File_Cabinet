using System;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// FileCabinetCustomService.
    /// </summary>
    /// <seealso cref="FileCabinetApp.FileCabinetService" />
    public class FileCabinetCustomService : FileCabinetService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetCustomService"/> class.
        /// </summary>
        /// <param name="validator">validator.</param>
        public FileCabinetCustomService()
            : base(new CustomValidator())
        {
        }
    }
}