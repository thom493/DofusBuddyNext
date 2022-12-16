using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DofusBuddy.Managers;
using DofusBuddy.Settings;
using DofusBuddy.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DofusBuddy.ViewModels
{
    public class MainPageViewModel : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;

        public CharacterManager CharacterManager { get; set; }

        private IRelayCommand<bool> _toggleReplicateMouseClicksCommand;
        public IRelayCommand<bool> ToggleReplicateMouseClicksCommand
        {
            get => _toggleReplicateMouseClicksCommand;
            set => SetProperty(ref _toggleReplicateMouseClicksCommand, value);
        }

        private IRelayCommand _toggleSingleReplicateMouseClicks;
        public IRelayCommand ToggleSingleReplicateMouseClicks
        {
            get => _toggleSingleReplicateMouseClicks;
            set => SetProperty(ref _toggleSingleReplicateMouseClicks, value);
        }

        private IRelayCommand _displayAddCharacterViewCommand;
        public IRelayCommand DisplayAddCharacterViewCommand
        {
            get => _displayAddCharacterViewCommand;
            set => SetProperty(ref _displayAddCharacterViewCommand, value);
        }

        private IRelayCommand _displaySettingsViewCommand;
        public IRelayCommand DisplaySettingsViewCommand
        {
            get => _displaySettingsViewCommand;
            set => SetProperty(ref _displaySettingsViewCommand, value);
        }

        private IRelayCommand _refreshActiveCharactersCommand;
        public IRelayCommand RefreshActiveCharactersCommand
        {
            get => _refreshActiveCharactersCommand;
            set => SetProperty(ref _refreshActiveCharactersCommand, value);
        }

        private ApplicationSettings _applicationSettings;

        public ApplicationSettings ApplicationSettings
        {
            get => _applicationSettings;
            set => SetProperty(ref _applicationSettings, value);
        }

        public MainPageViewModel(IServiceProvider provider, IOptions<ApplicationSettings> options, CharacterManager characterManager, GameManager gameManager)
        {
            _serviceProvider = provider;
            ApplicationSettings = options.Value;
            CharacterManager = characterManager;
            ToggleReplicateMouseClicksCommand = new RelayCommand<bool>(x => gameManager.ToggleReplicateMouseClicks(x));
            ToggleSingleReplicateMouseClicks = new RelayCommand(gameManager.ToggleSingleReplicateMouseClicks);
            DisplayAddCharacterViewCommand = new RelayCommand(() => _serviceProvider.GetService<AddCharacterView>().Show());
            DisplaySettingsViewCommand = new RelayCommand(() => _serviceProvider.GetService<SettingsView>().Show());
            RefreshActiveCharactersCommand = new RelayCommand(characterManager.RefreshActiveCharacters);
        }
    }
}
