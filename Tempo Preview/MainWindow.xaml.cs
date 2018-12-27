namespace Tempo_Preview
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public readonly TpViewModel TpViewModel = new TpViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = TpViewModel;
        }
    }
}
