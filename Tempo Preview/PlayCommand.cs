using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Tempo_Preview
{
    public class PlayCommand : ICommand
    {
        private readonly TpViewModel _tpViewModel;
        private double[] _beats;
        private bool _isPlaying;

        private BackgroundBeatsPlayer _backgroundBeatsPlayer;
        private EventHandler _canExecuteChanged;

        public PlayCommand(TpViewModel tpViewModel)
        {
            _tpViewModel = tpViewModel;
        }

        public bool CanExecute(object parameter)
        {
            return !_isPlaying && !string.IsNullOrEmpty(_tpViewModel.BeatsText);
        }

        public async void Execute(object parameter)
        {
            if (_isPlaying)
                return;

            try
            {
                _beats = _tpViewModel.Beats;
                await PlayBeats();
            }
            catch (FormatException e)
            {
                MessageBox.Show($"Cannot parse beats. Please double check your entry.\n\n\nError message:\n\n{e.GetBaseException().Message}", "Fail to play",
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
                if (_canExecuteChanged != null)
                {
                    _canExecuteChanged -= value;
                }

                CommandManager.RequerySuggested -= value;
            }
        }

        private void RaiseCanExecuteChanged()
        {
            if (!_isPlaying)
                OnCanExecuteChanged();
        }

        protected virtual void OnCanExecuteChanged()
            => _canExecuteChanged?.Invoke(this, EventArgs.Empty);

        private async Task PlayBeats()
        {
            _isPlaying = true;
            RaiseCanExecuteChanged();

            _backgroundBeatsPlayer = new BackgroundBeatsPlayer(_tpViewModel.PlaybackSpeed, _beats);
            await _backgroundBeatsPlayer.Play();
            _backgroundBeatsPlayer.Dispose();

            _isPlaying = false;
            RaiseCanExecuteChanged();
        }
    }
}
