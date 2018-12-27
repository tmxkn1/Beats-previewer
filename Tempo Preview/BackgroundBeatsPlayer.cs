using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Media;
using System.Threading.Tasks;
using Tempo_Preview.PrecisionDelay;

namespace Tempo_Preview
{
    public class BackgroundBeatsPlayer:IDisposable
    {
        private readonly BackgroundWorker _backgroundWorker = new BackgroundWorker();
        private readonly SoundPlayer _beatSoundPlayer = new SoundPlayer(Properties.Resources.metronome_beat);
        private readonly DelayHandler _delayHandler = new DelayHandler();

        private readonly double _playbackSpeed;
        private readonly double[] _beats;

        private const int BaseSpeed = 1000;

        public BackgroundBeatsPlayer(double playbackSpeed, double[] beats)
        {
            _playbackSpeed = playbackSpeed;
            _beats = beats;
            _backgroundWorker.DoWork += _backgroundWorker_DoWork;
        }

        public async Task Play()
        {
            _backgroundWorker.RunWorkerAsync();

            while (_backgroundWorker.IsBusy)
            {
                await Task.Delay(1);
            }
        }

        private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            _beatSoundPlayer.Load();
            var sw = new Stopwatch();
            sw.Start();
            foreach (var beat in _beats)
            {
                var startTime = sw.ElapsedMilliseconds;
                PlayABeat();
                Debug.WriteLine($"PlayAbeat() time cost {sw.ElapsedMilliseconds - startTime}");
                var timeDelay = BeatToTimeDelay(beat);
                _delayHandler.Delay(new TimeSpan(0, 0, 0, 0, (int)(timeDelay - startTime + sw.ElapsedMilliseconds)));
                Debug.WriteLine($"Time delay: {timeDelay}, Start time: {startTime}, End time: {sw.ElapsedMilliseconds}, Time elapsed: {sw.ElapsedMilliseconds - startTime}");
            }
        }

        private int BeatToTimeDelay(double beat)
        {
            return (int)(beat * BaseSpeed * _playbackSpeed);
        }

        private void PlayABeat()
        {
            _beatSoundPlayer.Play();
        }

        public void Dispose()
        {
            _backgroundWorker?.Dispose();
        }
    }
}
