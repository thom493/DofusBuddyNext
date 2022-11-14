using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DofusBuddy.Core;
using DofusBuddy.Core.Settings;
using DofusBuddy.Models;
using DofusBuddy.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DofusBuddy.ViewModels
{
    public class MainPageViewModel : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly GameManager _gameManager;

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

        private IRelayCommand<bool> _toggleAutoSwitchOnFightTurnCommand;
        public IRelayCommand<bool> ToggleAutoSwitchOnFightTurnCommand
        {
            get => _toggleAutoSwitchOnFightTurnCommand;
            set => SetProperty(ref _toggleAutoSwitchOnFightTurnCommand, value);
        }

        private IRelayCommand _displayCharacterDetectionDialogCommand;
        public IRelayCommand DisplayCharacterDetectionDialogCommand
        {
            get => _displayCharacterDetectionDialogCommand;
            set => SetProperty(ref _displayCharacterDetectionDialogCommand, value);
        }

        private ApplicationSettings _applicationSettings;

        public ApplicationSettings ApplicationSettings
        {
            get => _applicationSettings;
            set => SetProperty(ref _applicationSettings, value);
        }

        private ObservableCollection<Character>? _characters;
        public ObservableCollection<Character>? Characters
        {
            get => _characters;
            set => SetProperty(ref _characters, value);
        }

        public MainPageViewModel(IServiceProvider provider, IOptions<ApplicationSettings> options, CharacterManager characterManager, GameManager gameManager)
        {
            _serviceProvider = provider;
            ApplicationSettings = options.Value;
            _gameManager = gameManager;
            Characters = characterManager.ActiveCharacters;
            ToggleReplicateMouseClicksCommand = new RelayCommand<bool>(x => _gameManager.ToggleReplicateMouseClicks(x));
            ToggleSingleReplicateMouseClicks = new RelayCommand(_gameManager.ToggleSingleReplicateMouseClicks);
            ToggleAutoSwitchOnFightTurnCommand = new RelayCommand<bool>(x => _gameManager.ToggleAutoSwitchOnFightTurn(x));
            DisplayCharacterDetectionDialogCommand = new RelayCommand(() => _serviceProvider.GetService<CharacterDetectionWindow>().ShowDialog());
        }
    }
}
