using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Tempo_Preview
{
    public class TpViewModel:INotifyPropertyChanged
    {
        public TpViewModel()
        {
            PlayCommand = new PlayCommand(this);
        }

        public string BeatsText { get; set; }

        public double PlaybackSpeed { get; set; } = .5;

        public double[] Beats
        {
            get
            {
                if (string.IsNullOrEmpty(BeatsText))
                {
                    return Array.Empty<double>();
                }

                var splittedText = BeatsText.Split(',');

                return splittedText.Select(double.Parse).ToArray();
            }
        }

        public ICommand PlayCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
