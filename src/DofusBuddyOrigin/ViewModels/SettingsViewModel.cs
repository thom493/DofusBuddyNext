using CommunityToolkit.Mvvm.ComponentModel;
using DofusBuddy.Settings;
using Microsoft.Extensions.Options;

namespace DofusBuddy.ViewModels
{
    public class SettingsViewModel : ObservableObject
    {
        private ApplicationSettings _applicationSettings;

        public ApplicationSettings ApplicationSettings
        {
            get => _applicationSettings;
            set => SetProperty(ref _applicationSettings, value);
        }

        public SettingsViewModel(IOptions<ApplicationSettings> options)
        {
            _applicationSettings = options.Value;
        }
    }
}
