using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DofusBuddy.Core.GameEvents;
using DofusBuddy.Core.Settings;
using DofusBuddy.Models;
using Gma.System.MouseKeyHook;
using Microsoft.Extensions.Options;
using PInvoke;
using SharpHook;

namespace DofusBuddy.Core.Managers
{
    public class GameManager
    {
        private readonly CharacterManager _characterManager;
        private readonly ApplicationSettings _applicationSettings;
        private readonly WindowManager _windowManager;
        private readonly HookManager _hookManager;
        private readonly PacketManager _packetManager;
        private readonly KeyboardManager _keyboardManager;

        public GameManager(IOptions<ApplicationSettings> options,
            CharacterManager characterManager,
            WindowManager windowManager,
            HookManager hookManager,
            PacketManager packetManager,
            KeyboardManager keyboardManager)
        {
            _applicationSettings = options.Value;
            _characterManager = characterManager;
            _windowManager = windowManager;
            _hookManager = hookManager;
            _packetManager = packetManager;
            _keyboardManager = keyboardManager;

            _packetManager.FightTurnPacketReceived += OnFightTurn;
            _packetManager.GroupInvitationReceived += OnGroupInvitation;
            _packetManager.TradeInvitationReceived += OnTradeInvitation;

            SetupKeyboardKeybindings();
        }

        public void ToggleReplicateMouseClicks(bool enabled)
        {
            foreach (Character character in _characterManager.ActiveCharacters)
            {
                character.Settings.ReplicateMouseClick = enabled;
            }

            if (enabled)
            {
                _hookManager.GlobalHook.MouseClicked += OnGameWindowClick;
            }
            else
            {
                _hookManager.GlobalHook.MouseClicked -= OnGameWindowClick;
            }
        }

        public void ToggleSingleReplicateMouseClicks()
        {
            if (_applicationSettings.Characters.Any(x => x.ReplicateMouseClick) && !_applicationSettings.Features.ReplicateMouseClicks)
            {
                _applicationSettings.Features.ReplicateMouseClicks = true;
                _hookManager.GlobalHook.MouseClicked += OnGameWindowClick;
            }
            else if (_applicationSettings.Characters.All(x => !x.ReplicateMouseClick) && _applicationSettings.Features.ReplicateMouseClicks)
            {
                _applicationSettings.Features.ReplicateMouseClicks = false;
                _hookManager.GlobalHook.MouseClicked -= OnGameWindowClick;
            }
        }

        private void OnGroupInvitation(object? sender, GroupInvitationEventArgs e)
        {
            if (_applicationSettings.Features.AutoAcceptGroupInvitation)
            {
                Character? senderCharacter = _characterManager.ActiveCharacters.FirstOrDefault(x => x.Settings.Name == e.SenderName);
                Character? receiverCharacter = _characterManager.ActiveCharacters.FirstOrDefault(x => x.Settings.Name == e.ReceiverName);

                if (senderCharacter is not null && receiverCharacter is not null)
                {
                    ClickOnAcceptButton(receiverCharacter);
                }
            }
        }

        private void OnTradeInvitation(object? sender, TradeInvitationEventArgs e)
        {
            if (_applicationSettings.Features.AutoAcceptTradeInvitation)
            {
                Character? senderCharacter = _characterManager.ActiveCharacters.FirstOrDefault(x => x.Settings.Id == e.SenderId);
                Character? receiverCharacter = _characterManager.ActiveCharacters.FirstOrDefault(x => x.Settings.Id == e.ReceiverId);

                if (senderCharacter is not null && receiverCharacter is not null)
                {
                    ClickOnAcceptButton(receiverCharacter);
                }
            }
        }

        private void OnFightTurn(object? sender, FightTurnEventArgs e)
        {
            Character? character = _characterManager.ActiveCharacters.FirstOrDefault(x => x.Settings.Id == e.CharacterId);
            if (character is not null)
            {
                if (_applicationSettings.Features.AutoSwitchOnFightTurn)
                {
                    _windowManager.SetForegroundWindow(character.Process.MainWindowHandle);
                }

                if (character.Settings.AutoSkipTurn)
                {
                    _windowManager.SetForegroundWindow(character.Process.MainWindowHandle);
                    _keyboardManager.SendSingleKeyPress(User32.ScanCode.SPACE);
                }
            }
        }

        private void ClickOnAcceptButton(Character receiverCharacter)
        {
            var windowInfo = new User32.WINDOWINFO();
            User32.GetWindowInfo(receiverCharacter.Process.MainWindowHandle, ref windowInfo);

            int windowX = windowInfo.rcClient.right - windowInfo.rcClient.left;
            int windowY = windowInfo.rcClient.bottom - windowInfo.rcClient.top;

            // Accept button positon: 50.78% of X axis, 40,27% of Y axis
            int acceptButtonX = (int)(windowX * 0.5078) + windowInfo.rcClient.left;
            int acceptButtonY = (int)(windowY * 0.4027) + windowInfo.rcClient.top;
            _windowManager.SendLeftClickToWindow(receiverCharacter.Process.MainWindowHandle, acceptButtonX, acceptButtonY);
        }

        private async void OnGameWindowClick(object? sender, MouseHookEventArgs e)
        {
            if (e.Data.Button != SharpHook.Native.MouseButton.Button1)
            {
                // Mouse click isn't on left mouse button
                return;
            }

            Character? foregroundCharacter = _characterManager.ActiveCharacters.FirstOrDefault(x => x.Process.MainWindowHandle == User32.GetForegroundWindow());
            if (foregroundCharacter is null)
            {
                // The foreground window isn't dofus
                return;
            }

            var windowInfo = new User32.WINDOWINFO();
            User32.GetWindowInfo(foregroundCharacter.Process.MainWindowHandle, ref windowInfo);

            foreach (Character character in _characterManager.ActiveCharacters.Where(x => x.Settings.ReplicateMouseClick && x.Settings.Name != foregroundCharacter.Settings.Name))
            {
                await Task.Delay(_applicationSettings.Features.ReplicateMouseClicksDelay);
                _windowManager.SendLeftClickToWindow(character.Process.MainWindowHandle, e.Data.X - windowInfo.rcClient.left, e.Data.Y - windowInfo.rcClient.top);
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
                bindings.Add(new KeyValuePair<Combination, Action>(combination, () => _windowManager.SetForegroundWindow(character.Process.MainWindowHandle)));
            }
        }
    }
}
