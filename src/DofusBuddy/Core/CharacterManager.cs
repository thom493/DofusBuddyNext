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

        public ObservableCollection<Character> ActiveCharacters { get; set; }

        public CharacterManager(IOptions<ApplicationSettings> options, WindowManager windowManager)
        {
            _applicationSettings = options.Value;
            _windowManager = windowManager;
            SetActiveCharacters();
        }

        public void AddCharacter(CharacterSettings characterSettings)
        {
            Process? process = GetCharacterProcess(characterSettings.Name);
            ActiveCharacters.Add(new Character(characterSettings, process!));
        }

        public void UpdateApplicationSettings(ApplicationSettings applicationSettings)
        {
            applicationSettings.Characters = ActiveCharacters
                .Select(x => x.Settings)
                .ToList();
        }

        private void SetActiveCharacters()
        {
            ActiveCharacters = new ObservableCollection<Character>();
            foreach (CharacterSettings character in _applicationSettings.Characters)
            {
                AddCharacter(character);
            }
        }

        private Process? GetCharacterProcess(string characterName)
        {
            Process? process = Process
                .GetProcessesByName("Dofus Retro")
                .SingleOrDefault(x => x.MainWindowHandle != default && _windowManager.GetCharacterNameFromProcessWindowTitle(x) == characterName);

            return process;
        }
    }
}
