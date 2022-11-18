using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DofusBuddy.Core.Settings
{
    public class CharacterSettings : ObservableObject
    {
        private string? _id;
        private string? _name;
        private string? _focusWindowKeyBinding;
        private bool _replicateMouseClick;
        private bool _autoSkipTurn;

        public string? Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string? Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string? FocusWindowKeyBinding
        {
            get => _focusWindowKeyBinding;
            set => SetProperty(ref _focusWindowKeyBinding, value);
        }

        public bool AutoSkipTurn
        {
            get => _autoSkipTurn;
            set => SetProperty(ref _autoSkipTurn, value);
        }

        [JsonIgnore]
        public bool ReplicateMouseClick
        {
            get => _replicateMouseClick;
            set => SetProperty(ref _replicateMouseClick, value);
        }
    }
}
