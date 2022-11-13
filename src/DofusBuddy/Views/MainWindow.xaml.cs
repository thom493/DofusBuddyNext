using System.ComponentModel;
using DofusBuddy.Core.Settings;
using Microsoft.Extensions.Options;
using Wpf.Ui.Controls;

namespace DofusBuddy.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : UiWindow
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

        private void UiWindow_Closing(object sender, CancelEventArgs e)
        {
            _applicationSettings.WindowPosition = new WindowPositionSettings
            {
                Top = Top,
                Left = Left,
                Width = Width
            };
        }
    }
}
