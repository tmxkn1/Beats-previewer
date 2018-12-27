using System;
using System.Diagnostics;
using System.Threading;

namespace Tempo_Preview.PrecisionDelay
{
    public class Spinner
    {
        private readonly Stopwatch _spinStopwatch = new Stopwatch();
        private SpinWait _spinner = new SpinWait();

        public TimeSpan GetSpinTime()
        {
            _spinStopwatch.Restart();
            _spinner.SpinOnce();
            _spinStopwatch.Stop();
            return _spinStopwatch.Elapsed;
        }
    }
}
