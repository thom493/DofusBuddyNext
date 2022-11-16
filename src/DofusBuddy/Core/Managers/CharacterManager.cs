using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using DofusBuddy.Core.Settings;
using DofusBuddy.Models;
using Microsoft.Extensions.Options;

namespace DofusBuddy.Core.Managers
{
    public class CharacterManager
    {
        private readonly ApplicationSettings _applicationSettings;

        public ObservableCollection<Character> ActiveCharacters { get; set; } = new ObservableCollection<Character>();

        public CharacterManager(IOptions<ApplicationSettings> options)
        {
            _applicationSettings = options.Value;
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
