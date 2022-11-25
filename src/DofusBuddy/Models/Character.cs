using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using DofusBuddy.Settings;

namespace DofusBuddy.Models
{
    public class Character : ObservableObject
    {
        public Character(CharacterSettings characterSettings, Process process)
        {
            Settings = characterSettings;
            Process = process;
        }

        public CharacterSettings Settings { get; set; }

        public Process Process { get; set; }
    }
}
