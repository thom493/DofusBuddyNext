using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DofusBuddy.Core.Settings;
using DofusBuddy.Models;
using Gma.System.MouseKeyHook;
using Microsoft.Extensions.Options;
using PInvoke;
using SharpHook;
using SharpHook.Native;

namespace DofusBuddy.Core
{
    public class HookManager
    {
        private readonly ApplicationSettings _applicationSettings;
        private readonly WindowManager _windowManager;
        private readonly IKeyboardMouseEvents _keyboardMouseEvents;
        private readonly TaskPoolGlobalHook _globalHook;

        public HookManager(IOptions<ApplicationSettings> options, WindowManager windowManager)
        {
            _applicationSettings = options.Value;
            _windowManager = windowManager;
            _keyboardMouseEvents = Hook.GlobalEvents();
            _globalHook = new TaskPoolGlobalHook();
        }

        /// <summary>
        /// - Gma.System.MouseKeyHook to setup keyboards hooks
        /// - SharpHook to setup mouse hooks
        /// </summary>
        public void SetupAndRunHooks(ICollection<Character> characters)
        {
            SetupKeyboardHooks(characters);
            SetupMouseHook(characters);
            Task.Run(() => _globalHook.Run());
        }

        private void SetupKeyboardHooks(ICollection<Character> characters)
        {
            var keyboardShortcuts = new List<KeyValuePair<Combination, Action>>();
            AddReplicateMouseClicksKeyBinding(keyboardShortcuts);
            AddFocusWindowKeyBindings(keyboardShortcuts, characters);
            _keyboardMouseEvents.OnCombination(keyboardShortcuts);
        }

        private void AddReplicateMouseClicksKeyBinding(List<KeyValuePair<Combination, Action>> bindings)
        {
            var combination = Combination.FromString(_applicationSettings.Features.ReplicateMouseClicksKeyBinding);
            Action action = () => { _applicationSettings.Features.ReplicateMouseClicks = !_applicationSettings.Features.ReplicateMouseClicks; };
            bindings.Add(new KeyValuePair<Combination, Action>(combination, action));
        }

        private void AddFocusWindowKeyBindings(List<KeyValuePair<Combination, Action>> bindings, ICollection<Character> characters)
        {
            foreach (Character character in characters.Where(x => !string.IsNullOrEmpty(x.Settings.FocusWindowKeyBinding)))
            {
                var combination = Combination.FromString(character.Settings.FocusWindowKeyBinding);
                Action action = () => _windowManager.SetForegroundWindow(character.Process.MainWindowHandle);
                bindings.Add(new KeyValuePair<Combination, Action>(combination, action));
            }
        }

        private void SetupMouseHook(ICollection<Character> characters)
        {
            _globalHook.MouseClicked += async (_, eventArgs) => await OnMouseClicked(_, eventArgs, characters);
        }

        private async Task OnMouseClicked(object? _, MouseHookEventArgs eventArgs, ICollection<Character> characters)
        {
            if (!_applicationSettings.Features.ReplicateMouseClicks || eventArgs.Data.Button != MouseButton.Button1)
            {
                // Feature isn't toggled on or mouse click isn't on left mouse button
                return;
            }

            Character? foregroundCharacter = characters.FirstOrDefault(x => x.Process.MainWindowHandle == User32.GetForegroundWindow());
            if (foregroundCharacter is null)
            {
                // The foreground window isn't dofus
                return;
            }

            foreach (Character? character in characters.Where(x => x.Settings.Name != foregroundCharacter.Settings.Name))
            {
                await Task.Delay(_applicationSettings.Features.ReplicateMouseClicksDelay);
                SendLeftClickToWindow(character.Process.MainWindowHandle, eventArgs.Data.X, eventArgs.Data.Y);
            }
        }

        private static void SendLeftClickToWindow(IntPtr windowHandle, int x, int y)
        {
            var lParam = (IntPtr)((y << 16) | (x & 0xffff));
            User32.PostMessage(windowHandle, User32.WindowMessage.WM_LBUTTONDOWN, new IntPtr(0x0001), lParam);
            User32.PostMessage(windowHandle, User32.WindowMessage.WM_LBUTTONUP, new IntPtr(0x0000), lParam);
        }
    }
}
