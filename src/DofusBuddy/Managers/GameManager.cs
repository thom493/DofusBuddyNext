using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DofusBuddy.GameEvents;
using DofusBuddy.Models;
using DofusBuddy.Settings;
using Gma.System.MouseKeyHook;
using Microsoft.Extensions.Options;
using PInvoke;
using SharpHook;

namespace DofusBuddy.Managers
{
    public class GameManager
    {
        private readonly CharacterManager _characterManager;
        private readonly ApplicationSettings _applicationSettings;
        private readonly WindowManager _windowManager;
        private readonly HookManager _hookManager;
        private readonly PacketManager _packetManager;

        public GameManager(IOptions<ApplicationSettings> options,
            CharacterManager characterManager,
            WindowManager windowManager,
            HookManager hookManager,
            PacketManager packetManager)
        {
            _applicationSettings = options.Value;
            _characterManager = characterManager;
            _windowManager = windowManager;
            _hookManager = hookManager;
            _packetManager = packetManager;

            _packetManager.FightTurnPacketReceived += OnFightTurn;
            _packetManager.GroupInvitationReceived += OnGroupInvitation;
            _packetManager.TradeInvitationReceived += OnTradeInvitation;
            _hookManager.GlobalHook.MouseClicked += OnMouseClick;

            SetupKeyboardKeybindings();
        }

        public void ToggleReplicateMouseClicks(bool enabled)
        {
            foreach (Character character in _characterManager.ActiveCharacters)
            {
                character.Settings.ReplicateMouseClick = enabled;
            }
        }

        public void ToggleSingleReplicateMouseClicks()
        {
            _applicationSettings.Features.ReplicateLeftMouseClicks = _applicationSettings.Characters.Any(x => x.ReplicateMouseClick);
        }

        private async void OnGroupInvitation(object? sender, GroupInvitationEventArgs e)
        {
            if (_applicationSettings.Features.AutoAcceptGroupInvitation)
            {
                Character? senderCharacter = _characterManager.ActiveCharacters.FirstOrDefault(x => x.Settings.Name == e.SenderName);
                Character? receiverCharacter = _characterManager.ActiveCharacters.FirstOrDefault(x => x.Settings.Name == e.ReceiverName);

                if (senderCharacter is not null && receiverCharacter is not null)
                {
                    await Task.Delay(100);
                    _windowManager.SendLeftClickToWindow(receiverCharacter.Process.MainWindowHandle, 0.4375, 0.4375);
                }
            }
        }

        private async void OnTradeInvitation(object? sender, TradeInvitationEventArgs e)
        {
            if (_applicationSettings.Features.AutoAcceptTradeInvitation)
            {
                Character? senderCharacter = _characterManager.ActiveCharacters.FirstOrDefault(x => x.Settings.Id == e.SenderId);
                Character? receiverCharacter = _characterManager.ActiveCharacters.FirstOrDefault(x => x.Settings.Id == e.ReceiverId);

                if (senderCharacter is not null && receiverCharacter is not null)
                {
                    await Task.Delay(100);
                    _windowManager.SendLeftClickToWindow(receiverCharacter.Process.MainWindowHandle, 0.4375, 0.4375);
                }
            }
        }

        private async void OnFightTurn(object? sender, FightTurnEventArgs e)
        {
            Character? character = _characterManager.ActiveCharacters.FirstOrDefault(x => x.Settings.Id == e.CharacterId);
            if (character is not null)
            {
                if (character.Settings.AutoSkipTurn)
                {
                    await Task.Delay(500);
                    _windowManager.SendLeftClickToWindow(character.Process.MainWindowHandle, 0.6170, 0.9440);
                }
                else if (_applicationSettings.Features.AutoSwitchOnFightTurn)
                {
                    _windowManager.SetForegroundWindow(character.Process.MainWindowHandle);
                }
            }
        }

        private async void OnMouseClick(object? sender, MouseHookEventArgs e)
        {
            if (e.Data.Button == SharpHook.Native.MouseButton.Button1)
            {
                await ReplicateMouseClickOnSpecificCharactersWithDelay(e.Data.X, e.Data.Y);
            }
            else if (e.Data.Button == SharpHook.Native.MouseButton.Button3)
            {
                ReplicateMouseClickOnEveryCharacterWithoutDelay(e.Data.X, e.Data.Y);
            }
        }

        private async Task ReplicateMouseClickOnSpecificCharactersWithDelay(int x, int y)
        {
            if (!_applicationSettings.Features.ReplicateLeftMouseClicks)
            {
                return;
            }

            if (!IsForegroundWindowDofus(out Character foregroundCharacter))
            {
                return;
            }

            User32.WINDOWINFO windowInfo = _windowManager.GetWindowInfo(foregroundCharacter.Process.MainWindowHandle);
            foreach (Character character in _characterManager.ActiveCharacters.Where(x => x.Settings.ReplicateMouseClick && x.Settings.Id != foregroundCharacter.Settings.Id))
            {
                await Task.Delay(_applicationSettings.Features.ReplicateLeftMouseClicksDelay);
                _windowManager.SendLeftClickToWindow(character.Process.MainWindowHandle, x - windowInfo.rcClient.left, y - windowInfo.rcClient.top);
            }
        }

        private void ReplicateMouseClickOnEveryCharacterWithoutDelay(int x, int y)
        {
            if (!_applicationSettings.Features.LeftMouseClickOnWheelClick)
            {
                return;
            }

            if (!IsForegroundWindowDofus(out Character foregroundCharacter))
            {
                return;
            }

            User32.WINDOWINFO windowInfo = _windowManager.GetWindowInfo(foregroundCharacter.Process.MainWindowHandle);
            foreach (Character character in _characterManager.ActiveCharacters)
            {
                _windowManager.SendLeftClickToWindow(character.Process.MainWindowHandle, x - windowInfo.rcClient.left, y - windowInfo.rcClient.top);
            }
        }

        private bool IsForegroundWindowDofus(out Character foregroundCharacter)
        {
            foregroundCharacter = _characterManager.ActiveCharacters.FirstOrDefault(x => x.Process.MainWindowHandle == User32.GetForegroundWindow());
            return foregroundCharacter is not null;
        }

        private void SetupKeyboardKeybindings()
        {
            var keyboardKeyBindings = new List<KeyValuePair<Combination, Action>>();
            AddReplicateMouseClicksKeyBinding(keyboardKeyBindings);
            _hookManager.KeyboardMouseEvents.OnCombination(keyboardKeyBindings);
        }

        private void AddReplicateMouseClicksKeyBinding(List<KeyValuePair<Combination, Action>> bindings)
        {
            var combination = Combination.FromString(_applicationSettings.Features.ReplicateMouseClicksKeyBinding);
            bindings.Add(new KeyValuePair<Combination, Action>(combination, action));

            void action()
            {
                _applicationSettings.Features.ReplicateLeftMouseClicks = !_applicationSettings.Features.ReplicateLeftMouseClicks;
                ToggleReplicateMouseClicks(_applicationSettings.Features.ReplicateLeftMouseClicks);
            }
        }
    }
}
