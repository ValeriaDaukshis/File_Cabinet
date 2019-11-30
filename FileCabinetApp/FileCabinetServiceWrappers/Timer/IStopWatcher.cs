namespace FileCabinetApp.Timer
{
    /// <summary>
    /// IStopWatcher.
    /// </summary>
    public interface IStopWatcher
    {
        /// <summary>
        /// Gets the ellapsed milliseconds.
        /// </summary>
        /// <value>
        /// The ellapsed milliseconds.
        /// </value>
        long EllapsedMilliseconds { get; }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        void StartTimer();

        /// <summary>
        /// Stops this instance.
        /// </summary>
        void StopTimer();
    }
}