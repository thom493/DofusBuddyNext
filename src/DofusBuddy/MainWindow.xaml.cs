using DofusBuddy.Settings;
using Microsoft.Extensions.Options;

namespace DofusBuddy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly ApplicationSettings _applicationSettings;

        public MainWindow(IOptions<ApplicationSettings> options, MainPage mainPage)
        {
            _applicationSettings = options.Value;

            InitializeComponent();

            SetMainWindowSizeAndPosition();

            MainFrame.Navigate(mainPage);
        }

        private void SetMainWindowSizeAndPosition()
        {
            if (_applicationSettings.WindowPosition is not null)
            {
                Top = _applicationSettings.WindowPosition.Top;
                Left = _applicationSettings.WindowPosition.Left;
                Width = _applicationSettings.WindowPosition.Width;
            }
        }
    }
}
