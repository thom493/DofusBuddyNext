using System.Diagnostics;
using DofusBuddy.Core.Settings;

namespace DofusBuddy.Core
{
    public class Character
    {
        public Character(CharacterSettings characterSettings, Process process)
        {
            CharacterSettings = characterSettings;
            Process = process;
        }

        public CharacterSettings CharacterSettings { get; set; }

        public Process Process { get; set; }
    }
}
