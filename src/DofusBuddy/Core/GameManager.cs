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
using SharpHook.Native;

namespace DofusBuddy.Core
{
    public class GameManager
    {
        private readonly CharacterManager _characterManager;
        private readonly ApplicationSettings _applicationSettings;
        private readonly WindowManager _windowManager;
        private readonly HookManager _hookManager;
        private readonly PacketManager _packetManager;

        public GameManager(IOptions<ApplicationSettings> options, CharacterManager characterManager, WindowManager windowManager, HookManager hookManager, PacketManager packetManager)
        {
            _applicationSettings = options.Value;
            _characterManager = characterManager;
            _windowManager = windowManager;
            _hookManager = hookManager;
            _packetManager = packetManager;
            SetupKeyboardKeybindings();
            ToggleAutoSwitchOnFightTurn(_applicationSettings.Features.AutoSwitchOnFightTurn);
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

        public void ToggleAutoSwitchOnFightTurn(bool enabled)
        {
            if (enabled)
            {
                _packetManager.FightTurnPacketReceived += OnFightTurn;
            }
            else
            {
                _packetManager.FightTurnPacketReceived -= OnFightTurn;
            }
        }

        private void OnFightTurn(object? sender, FightTurnEventArgs args)
        {
            Character? character = _characterManager.ActiveCharacters.FirstOrDefault(x => x.Settings.Id == args.CharacterId);
            if (character is not null)
            {
                DisplayCharacterWindow(character);
            }
        }

        private async void OnGameWindowClick(object? sender, MouseHookEventArgs eventArgs)
        {
            if (eventArgs.Data.Button != MouseButton.Button1)
            {
                // Mouse click isn't on left mouse button
                return;
            }

            Character? foregroundCharacter = _characterManager.ActiveCharacters.FirstOrDefault(x => x.Process?.MainWindowHandle == User32.GetForegroundWindow());
            if (foregroundCharacter is null)
            {
                // The foreground window isn't dofus
                return;
            }

            var windowInfo = new User32.WINDOWINFO();
            User32.GetWindowInfo(foregroundCharacter.Process.MainWindowHandle, ref windowInfo);

            foreach (Character character in _characterManager.ActiveCharacters.Where(x => x.Settings.ReplicateMouseClick && x.Process is not null && x.Settings.Name != foregroundCharacter.Settings.Name))
            {
                await Task.Delay(_applicationSettings.Features.ReplicateMouseClicksDelay);
                _windowManager.SendLeftClickToWindow(character.Process.MainWindowHandle, eventArgs.Data.X, eventArgs.Data.Y - windowInfo.rcClient.top);
            }
        }

        private void DisplayCharacterWindow(Character character)
        {
            if (character.Process != null)
            {
                _windowManager.SetForegroundWindow(character.Process.MainWindowHandle);
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
                bindings.Add(new KeyValuePair<Combination, Action>(combination, () => DisplayCharacterWindow(character)));
            }
        }
    }
}
