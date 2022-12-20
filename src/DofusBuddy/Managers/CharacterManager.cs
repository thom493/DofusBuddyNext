using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DofusBuddy.Models;
using DofusBuddy.Settings;
using Gma.System.MouseKeyHook;
using Microsoft.Extensions.Options;

namespace DofusBuddy.Managers
{
    public class CharacterManager
    {
        private readonly ApplicationSettings _applicationSettings;
        private readonly HookManager _hookManager;
        private readonly WindowManager _windowManager;

        public ObservableCollection<Character> ActiveCharacters { get; set; } = new ObservableCollection<Character>();

        public CharacterManager(IOptions<ApplicationSettings> options, HookManager hookManager, WindowManager windowManager)
        {
            _applicationSettings = options.Value;
            _hookManager = hookManager;
            _windowManager = windowManager;
            RefreshActiveCharacters();
        }

        public void AddCharacter(CharacterSettings characterSettings)
        {
            _applicationSettings.Characters.Add(characterSettings);
            TryAddActiveCharacter(characterSettings);
        }

        public void RefreshActiveCharacters()
        {
            ActiveCharacters.Clear();
            foreach (CharacterSettings characterSettings in _applicationSettings.Characters)
            {
                TryAddActiveCharacter(characterSettings);
            }
        }

        public async void ConnectFirstCharacterOfEveryGameWindow()
        {
            IEnumerable<Process> processes = Process
                .GetProcessesByName("Dofus Retro")
                .Where(x => x.MainWindowHandle != default && x.MainWindowTitle.StartsWith("Dofus Retro"));

            foreach (Process process in processes)
            {
                _windowManager.SetForegroundWindow(process.MainWindowHandle);

                _windowManager.SendLeftClickToWindow(process.MainWindowHandle, 0.2540, 0.5156);
                _windowManager.SendLeftClickToWindow(process.MainWindowHandle, 0.2540, 0.5156);

                await Task.Delay(2000);

                _windowManager.SendLeftClickToWindow(process.MainWindowHandle, 0.2540, 0.5156);
                _windowManager.SendLeftClickToWindow(process.MainWindowHandle, 0.2540, 0.5156);
            }

            await Task.Delay(2000);

            RefreshActiveCharacters();
        }

        private void TryAddActiveCharacter(CharacterSettings characterSettings)
        {
            Process? process = GetCharacterProcess(characterSettings.Name);
            if (process is not null)
            {
                if (!string.IsNullOrEmpty(characterSettings.FocusWindowKeyBinding))
                {
                    var combinations = new KeyValuePair<Combination, Action>[]
                    {
                        new KeyValuePair<Combination, Action>(Combination.FromString(characterSettings.FocusWindowKeyBinding), () => _windowManager.SetForegroundWindow(process.MainWindowHandle))
                    };

                    _hookManager.KeyboardMouseEvents.OnCombination(combinations);
                }

                ActiveCharacters.Add(new Character(characterSettings, process));
            }
        }

        private static Process? GetCharacterProcess(string? characterName)
        {
            Process? process = Process
                .GetProcessesByName("Dofus Retro")
                .SingleOrDefault(x => x.MainWindowHandle != default && GetCharacterNameFromProcessWindowTitle(x) == characterName);

            return process;
        }

        private static string GetCharacterNameFromProcessWindowTitle(Process process)
        {
            var regex = new Regex("(.*?) \\- ");
            return regex.Match(process.MainWindowTitle)
                .Groups[1]
                .Value;
        }
    }
}
