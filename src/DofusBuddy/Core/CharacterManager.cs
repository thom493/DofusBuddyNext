using System.Collections.Generic;
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

        public List<Character> ActiveCharacters { get; set; }

        public CharacterManager(IOptions<ApplicationSettings> options, WindowManager windowManager)
        {
            _applicationSettings = options.Value;
            _windowManager = windowManager;
            ActiveCharacters = GetActiveCharacters();
        }

        private List<Character> GetActiveCharacters()
        {
            List<Character> characters = new();

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
