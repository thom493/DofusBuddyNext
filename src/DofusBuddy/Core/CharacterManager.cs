using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using DofusBuddy.Core.Settings;
using DofusBuddy.Models;
using Microsoft.Extensions.Options;

namespace DofusBuddy.Core
{
    public class CharacterManager
    {
        private readonly ApplicationSettings _applicationSettings;
        private readonly WindowManager _windowManager;

        public ObservableCollection<Character> ActiveCharacters { get; set; } = new ObservableCollection<Character>();

        public CharacterManager(IOptions<ApplicationSettings> options, WindowManager windowManager)
        {
            _applicationSettings = options.Value;
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

        private void TryAddActiveCharacter(CharacterSettings characterSettings)
        {
            Process? process = GetCharacterProcess(characterSettings.Name);
            if (process is not null)
            {
                ActiveCharacters.Add(new Character(characterSettings, process));
            }
        }

        private Process? GetCharacterProcess(string? characterName)
        {
            Process? process = Process
                .GetProcessesByName("Dofus Retro")
                .SingleOrDefault(x => x.MainWindowHandle != default && _windowManager.GetCharacterNameFromProcessWindowTitle(x) == characterName);

            return process;
        }
    }
}
