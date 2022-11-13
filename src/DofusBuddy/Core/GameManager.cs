using System.Linq;
using System.Threading.Tasks;
using DofusBuddy.Core.GameEvents;
using DofusBuddy.Core.Settings;
using DofusBuddy.Models;
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

        public GameManager(IOptions<ApplicationSettings> options, CharacterManager characterManager, WindowManager windowManager)
        {
            _applicationSettings = options.Value;
            _characterManager = characterManager;
            _windowManager = windowManager;
        }

        public void DisplayCharacterWindow(Character character)
        {
            _windowManager.SetForegroundWindow(character.Process.MainWindowHandle);
        }

        public void OnFightTurn(object? sender, FightTurnEventArgs args)
        {
            Character? character = _characterManager.ActiveCharacters.FirstOrDefault(x => x.Settings.Id == args.CharacterId);
            if (character is not null)
            {
                DisplayCharacterWindow(character);
            }
        }

        public async void OnGameWindowClick(object? sender, MouseHookEventArgs eventArgs)
        {
            if (eventArgs.Data.Button != MouseButton.Button1)
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

            foreach (Character character in _characterManager.ActiveCharacters.Where(x => x.Process is not null && x.Settings.Name != foregroundCharacter.Settings.Name))
            {
                await Task.Delay(_applicationSettings.Features.ReplicateMouseClicksDelay);
                _windowManager.SendLeftClickToWindow(character.Process.MainWindowHandle, eventArgs.Data.X, eventArgs.Data.Y - windowInfo.rcClient.top);
            }
        }
    }
}
