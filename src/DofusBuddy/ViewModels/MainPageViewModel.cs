using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using DofusBuddy.Core.Settings;
using DofusBuddy.Models;
using Gma.System.MouseKeyHook;
using Microsoft.Extensions.Options;
using PInvoke;

namespace DofusBuddy.ViewModels
{
    public class MainPageViewModel
    {
        private readonly ApplicationSettings _applicationSettings;
        private readonly IKeyboardMouseEvents _keyboardMouseEvents;

        public ObservableCollection<Character> Characters { get; set; }

        public MainPageViewModel(IOptions<ApplicationSettings> options)
        {
            _applicationSettings = options.Value;
            _keyboardMouseEvents = Hook.GlobalEvents();
            Characters = GetActiveCharacters();
            SetupKeyboardShortcuts();
        }

        private ObservableCollection<Character> GetActiveCharacters()
        {
            ObservableCollection<Character> characters = new();

            Process[] dofusProcesses = Process
                    .GetProcessesByName("Dofus Retro")
                    .Where(x => x.MainWindowHandle != default)
                    .ToArray();

            foreach (Process process in dofusProcesses)
            {
                CharacterSettings? settings = _applicationSettings.Characters.FirstOrDefault(x => x.Name == GetCharacterNameFromProcessWindowTitle(process));

                if (settings is not null)
                {
                    characters.Add(new Character(settings, process));
                }
            }

            return characters;
        }

        private void SetupKeyboardShortcuts()
        {
            var keyboardShortcuts = new List<KeyValuePair<Combination, Action>>();
            foreach (Character character in Characters.Where(x => !string.IsNullOrEmpty(x.Settings.FocusWindowKeyBinding)))
            {
                var combinationWithAction = new KeyValuePair<Combination, Action>(Combination.FromString(character.Settings.FocusWindowKeyBinding), () => SetForegroundWindow(character));
                keyboardShortcuts.Add(combinationWithAction);
            }

            _keyboardMouseEvents.OnCombination(keyboardShortcuts);
        }

        private static void SetForegroundWindow(Character character)
        {
            if (character.Process.MainWindowHandle != User32.GetForegroundWindow())
            {
                User32.SetForegroundWindow(character.Process.MainWindowHandle);
            }
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
