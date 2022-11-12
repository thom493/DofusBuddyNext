using System.Collections.ObjectModel;
using DofusBuddy.Core;
using DofusBuddy.Core.Settings;
using DofusBuddy.Models;
using Microsoft.Extensions.Options;

namespace DofusBuddy.ViewModels
{
    public class MainPageViewModel
    {
        private readonly HookManager _hookManager;
        private readonly CharacterManager _characterManager;

        public ApplicationSettings ApplicationSettings { get; set; }

        public ObservableCollection<Character> Characters { get; set; }

        public MainPageViewModel(IOptions<ApplicationSettings> options, HookManager hookManager, CharacterManager characterManager)
        {
            _hookManager = hookManager;
            _characterManager = characterManager;
            ApplicationSettings = options.Value;
            Characters = _characterManager.GetActiveCharacters();
            _hookManager.SetupAndRunHooks(Characters);
        }
    }
}
