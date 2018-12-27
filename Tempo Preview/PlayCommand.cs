using System;
using System.Diagnostics;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Tempo_Preview
{
    public class PlayCommand : ICommand
    {
        private readonly TpViewModel _tpViewModel;
        private double[] _beats;
        private bool IsPlaying;
        private const int BaseSpeed = 1000;

        private readonly SoundPlayer _beatSoundPlayer = new SoundPlayer(Properties.Resources.metronome_beat);
        private EventHandler _canExecuteChanged;

        public PlayCommand(TpViewModel tpViewModel)
        {
            _tpViewModel = tpViewModel;
        }

        public bool CanExecute(object parameter)
        {
            return !IsPlaying && !string.IsNullOrEmpty(_tpViewModel.BeatsText);
        }

        public async void Execute(object parameter)
        {
            if (IsPlaying)
                return;

            try
            {
                _beats = _tpViewModel.Beats;
                await PlayBeats();
            }
            catch (FormatException e)
            {
                MessageBox.Show($"Cannot parse beats. Please double check your entry.\n\n {e.GetBaseException().Message}", "Fail to play",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                _canExecuteChanged += value;
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                _canExecuteChanged -= value;
                CommandManager.RequerySuggested -= value;
            }
        }

        public void RaiseCanExecuteChanged()
        {
            if (!IsPlaying)
                OnCanExecuteChanged();
        }

        protected virtual void OnCanExecuteChanged()
            => _canExecuteChanged?.Invoke(this, EventArgs.Empty);

        private async Task PlayBeats()
        {
            IsPlaying = true;
            CommandManager.InvalidateRequerySuggested();
            _beatSoundPlayer.Load();
            var sw = new Stopwatch();
            sw.Start();
            foreach (var beat in _beats)
            {
                var startTime = sw.ElapsedMilliseconds;
                PlayABeat();
                Debug.WriteLine($"Time elpased {sw.ElapsedMilliseconds - startTime}");
                var timeDelay = BeatToTimeDelay(beat);
                await Task.Delay((int)(timeDelay + sw.ElapsedMilliseconds - startTime));
                Debug.WriteLine($"Time delay: {timeDelay}, Start time: {startTime}, endTime = {sw.ElapsedMilliseconds}, elapsed = {sw.ElapsedMilliseconds - startTime}");
            }

            IsPlaying = false;
            RaiseCanExecuteChanged();
        }

        private int BeatToTimeDelay(double beat)
        {
            return (int)(beat * BaseSpeed * _tpViewModel.PlaybackSpeed);
        }

        private void PlayABeat()
        {
            _beatSoundPlayer.Play();
        }
    }
}
