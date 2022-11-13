using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DofusBuddy.Core;
using DofusBuddy.Core.Settings;
using DofusBuddy.Models;
using DofusBuddy.Views;
using Gma.System.MouseKeyHook;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DofusBuddy.ViewModels
{
    public class MainPageViewModel : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly HookManager _hookManager;
        private readonly CharacterManager _characterManager;
        private readonly PacketManager _packetManager;
        private readonly GameManager _gameManager;

        private IRelayCommand<bool> _toggleReplicateMouseClicksCommand;
        public IRelayCommand<bool> ToggleReplicateMouseClicksCommand
        {
            get => _toggleReplicateMouseClicksCommand;
            set => SetProperty(ref _toggleReplicateMouseClicksCommand, value);
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

        public MainPageViewModel(IServiceProvider provider,
            IOptions<ApplicationSettings> options,
            HookManager hookManager,
            CharacterManager characterManager,
            PacketManager packetManager,
            GameManager gameManager)
        {
            _serviceProvider = provider;
            ApplicationSettings = options.Value;
            _hookManager = hookManager;
            _characterManager = characterManager;
            _packetManager = packetManager;
            _gameManager = gameManager;
            ToggleReplicateMouseClicksCommand = new RelayCommand<bool>(ToggleReplicateMouseClicks);
            ToggleAutoSwitchOnFightTurnCommand = new RelayCommand<bool>(ToggleAutoSwitchOnFightTurn);
            DisplayCharacterDetectionDialogCommand = new RelayCommand(DisplayCharacterDetectionDialog);
        }

        public void Initialize()
        {
            Characters = _characterManager.ActiveCharacters;
            _hookManager.Initialize();
            _packetManager.Initialize();

            SetupKeyboardKeybindings();

            if (ApplicationSettings.Features.AutoSwitchOnFightTurn)
            {
                _packetManager.FightTurnPacketReceived += _gameManager.OnFightTurn;
            }
        }

        private void SetupKeyboardKeybindings()
        {
            var keyboardKeyBindings = new List<KeyValuePair<Combination, Action>>();
            AddReplicateMouseClicksKeyBinding(keyboardKeyBindings);
            AddFocusWindowKeyBinding(keyboardKeyBindings);
            _hookManager.KeyboardMouseEvents.OnCombination(keyboardKeyBindings);
        }

        private void AddReplicateMouseClicksKeyBinding(List<KeyValuePair<Combination, Action>> bindings)
        {
            var combination = Combination.FromString(_applicationSettings.Features.ReplicateMouseClicksKeyBinding);
            bindings.Add(new KeyValuePair<Combination, Action>(combination, action));

            void action()
            {
                _applicationSettings.Features.ReplicateMouseClicks = !_applicationSettings.Features.ReplicateMouseClicks;
                ToggleReplicateMouseClicks(_applicationSettings.Features.ReplicateMouseClicks);
            }
        }

        private void AddFocusWindowKeyBinding(List<KeyValuePair<Combination, Action>> bindings)
        {
            foreach (Character character in _characterManager.ActiveCharacters.Where(x => !string.IsNullOrEmpty(x.Settings.FocusWindowKeyBinding)))
            {
                var combination = Combination.FromString(character.Settings.FocusWindowKeyBinding);
                bindings.Add(new KeyValuePair<Combination, Action>(combination, action));

                void action() => _gameManager.DisplayCharacterWindow(character);
            }
        }

        private void ToggleReplicateMouseClicks(bool enable)
        {
            if (enable)
            {
                _hookManager.GlobalHook.MouseClicked += _gameManager.OnGameWindowClick;
            }
            else
            {
                _hookManager.GlobalHook.MouseClicked -= _gameManager.OnGameWindowClick;
            }
        }

        private void ToggleAutoSwitchOnFightTurn(bool enable)
        {
            if (enable)
            {
                _packetManager.FightTurnPacketReceived += _gameManager.OnFightTurn;
            }
            else
            {
                _packetManager.FightTurnPacketReceived -= _gameManager.OnFightTurn;
            }
        }

        private void DisplayCharacterDetectionDialog()
        {
            _serviceProvider.GetService<CharacterDetectionWindow>().ShowDialog();
        }
    }
}
