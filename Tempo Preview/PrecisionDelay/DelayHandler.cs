using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Tempo_Preview.PrecisionDelay
{
    public class DelayHandler
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly Spinner _spinner = new Spinner();
        private TimeSpan _average;
        
        public void Delay(TimeSpan time)
        {
            _stopwatch.Restart();
            MonitorDelay(time);
        }

        private void MonitorDelay(TimeSpan time)
        {
            if (_average.Ticks == 0)
                _average = _spinner.GetSpinTime();
            while (_stopwatch.Elapsed < time)
            {
                if (_stopwatch.Elapsed + _average < time)
                {
                    _average = TimeSpan.FromTicks((_average + _spinner.GetSpinTime()).Ticks / 2);
                }
            }
        }
    }
}
