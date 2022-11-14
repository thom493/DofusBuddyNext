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

        public ObservableCollection<Character> InactiveCharacters { get; set; } = new ObservableCollection<Character>();

        public ObservableCollection<Character> ActiveCharacters { get; set; } = new ObservableCollection<Character>();

        public CharacterManager(IOptions<ApplicationSettings> options, WindowManager windowManager)
        {
            _applicationSettings = options.Value;
            _windowManager = windowManager;
            SetCharacters();
        }

        public void AddCharacter(CharacterSettings characterSettings)
        {
            Process? process = GetCharacterProcess(characterSettings.Name);
            if (process is not null)
            {
                ActiveCharacters.Add(new Character(characterSettings, process));
            }
            else
            {
                InactiveCharacters.Add(new Character(characterSettings, null));
            }
        }

        public void UpdateApplicationSettings(ApplicationSettings applicationSettings)
        {
            applicationSettings.Characters = ActiveCharacters
                .Select(x => x.Settings)
                .ToList();
        }

        private void SetCharacters()
        {
            foreach (CharacterSettings characterSettings in _applicationSettings.Characters)
            {
                AddCharacter(characterSettings);
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
