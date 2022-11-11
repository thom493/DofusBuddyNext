using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using DofusBuddy.Core.Settings;
using Gma.System.MouseKeyHook;
using Microsoft.Extensions.Options;
using PInvoke;

namespace DofusBuddy.Core
{
    public class MultiAccountManager
    {
        private readonly ApplicationSettings _applicationSettings;
        private readonly List<Character> _characters = new();

        public MultiAccountManager(IOptions<ApplicationSettings> options)
        {
            _applicationSettings = options.Value;
            LinkCharacterWithProcesses();
            SetupKeyboardShortcuts();
        }

        private void SetupKeyboardShortcuts()
        {
            var keyboardShortcuts = new List<KeyValuePair<Combination, Action>>();
            for (int i = 0; i < _characters.Count; i++)
            {
                Character character = _characters[i];
                var combinationWithAction = new KeyValuePair<Combination, Action>(Combination.FromString($"F{i + 1}"), () => SetForegroundWindow(character));
                keyboardShortcuts.Add(combinationWithAction);
            }

            Hook.GlobalEvents().OnCombination(keyboardShortcuts);
        }

        private static void SetForegroundWindow(Character character)
        {
            if (character.Process.MainWindowHandle != User32.GetForegroundWindow())
            {
                User32.SetForegroundWindow(character.Process.MainWindowHandle);
            }
        }

        private void LinkCharacterWithProcesses()
        {
            Process[] dofusProcesses = Process
                .GetProcessesByName("Dofus Retro")
                .Where(x => x.MainWindowHandle != default)
                .ToArray();

            foreach (Process process in dofusProcesses)
            {
                CharacterSettings? character = _applicationSettings.Characters.FirstOrDefault(x => x.Name == GetCharacterNameFromProcessWindowTitle(process));

                if (character is not null)
                {
                    _characters.Add(new Character(character, process));
                }
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
