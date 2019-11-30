using System.Diagnostics;

namespace FileCabinetApp.Timer
{
    /// <summary>
    /// StopWatcher.
    /// </summary>
    /// <seealso cref="FileCabinetApp.Timer.IStopWatcher" />
    public class StopWatcher : IStopWatcher
    {
        private readonly Stopwatch timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="StopWatcher"/> class.
        /// </summary>
        public StopWatcher()
        {
            this.timer = new Stopwatch();
        }

        /// <summary>
        /// Gets the ellapsed milliseconds.
        /// </summary>
        /// <value>
        /// The ellapsed milliseconds.
        /// </value>
        public long EllapsedMilliseconds => this.timer.ElapsedTicks;

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void StartTimer()
        {
            this.timer.Restart();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void StopTimer()
        {
            this.timer.Stop();
        }
    }
}