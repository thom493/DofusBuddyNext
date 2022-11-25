using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DofusBuddy.GameEvents;
using DofusBuddy.Managers;
using DofusBuddy.Models;
using DofusBuddy.Settings;

namespace DofusBuddy.ViewModels
{
    public class AddCharacterViewModel : ObservableObject, IDisposable
    {
        private readonly PacketManager _packetManager;
        private readonly CharacterManager _characterManager;
        private ObservableCollection<DetectedCharcter> _detectedCharacters;
        public ObservableCollection<DetectedCharcter> DetectedCharacters
        {
            get => _detectedCharacters;
            set => SetProperty(ref _detectedCharacters, value);
        }

        private IRelayCommand<DetectedCharcter> _removeCharacterCommand;
        public IRelayCommand<DetectedCharcter> RemoveCharacterCommand
        {
            get => _removeCharacterCommand;
            set => SetProperty(ref _removeCharacterCommand, value);
        }

        private IRelayCommand<DetectedCharcter> _addCharacterCommand;
        public IRelayCommand<DetectedCharcter> AddCharacterCommand
        {
            get => _addCharacterCommand;
            set => SetProperty(ref _addCharacterCommand, value);
        }

        public AddCharacterViewModel(PacketManager packetManager, CharacterManager characterManager)
        {
            _packetManager = packetManager;
            _characterManager = characterManager;
            _detectedCharacters = new ObservableCollection<DetectedCharcter>();
            RemoveCharacterCommand = new RelayCommand<DetectedCharcter>(RemoveCharacter);
            AddCharacterCommand = new RelayCommand<DetectedCharcter>(AddCharacter);
        }

        public void Initialize()
        {
            _packetManager.ChatMessagePacketReceived += OnChatMessage;
        }

        public void Dispose()
        {
            _packetManager.ChatMessagePacketReceived -= OnChatMessage;
        }

        private void OnChatMessage(object? sender, ChatMessageEventArgs e)
        {
            if (DetectedCharacters.All(x => x.Id != e.CharacterId))
            {
                // Avoid "This type of CollectionView does not support changes to its SourceCollection from a thread different from the Dispatcher thread" exception
                App.Current.Dispatcher.Invoke(() => DetectedCharacters.Add(new DetectedCharcter(e.CharacterId, e.CharacterName)));
            }
        }

        private void RemoveCharacter(DetectedCharcter detectedCharacter)
        {
            DetectedCharacters.Remove(detectedCharacter);
        }

        private void AddCharacter(DetectedCharcter detectedCharacter)
        {
            _characterManager.AddCharacter(new CharacterSettings { Id = detectedCharacter.Id, Name = detectedCharacter.Name });
            RemoveCharacter(detectedCharacter);
        }
    }
}
