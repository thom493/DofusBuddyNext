using DofusBuddy.ViewModels;
using Wpf.Ui.Controls;

namespace DofusBuddy.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UiWindow
    {
        public SettingsView(SettingsViewModel settingsViewModel)
        {
            InitializeComponent();
            DataContext = settingsViewModel;
        }
    }
}
