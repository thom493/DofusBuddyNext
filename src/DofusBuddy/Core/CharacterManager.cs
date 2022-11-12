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

        public CharacterManager(IOptions<ApplicationSettings> options, WindowManager windowManager)
        {
            _applicationSettings = options.Value;
            _windowManager = windowManager;

        }

        public ObservableCollection<Character> GetActiveCharacters()
        {
            ObservableCollection<Character> characters = new();

            Process[] dofusProcesses = Process
                .GetProcessesByName("Dofus Retro")
                .Where(x => x.MainWindowHandle != default)
                .ToArray();

            foreach (Process process in dofusProcesses)
            {
                CharacterSettings? settings = _applicationSettings.Characters.FirstOrDefault(x => x.Name == _windowManager.GetCharacterNameFromProcessWindowTitle(process));

                if (settings is not null)
                {
                    characters.Add(new Character(settings, process));
                }
            }

            return characters;
        }
    }
}
