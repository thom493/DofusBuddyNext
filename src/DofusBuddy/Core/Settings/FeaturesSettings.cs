using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DofusBuddy.Core.Settings
{
    public class FeaturesSettings : ObservableObject
    {
        private bool _replicateMouseClicks;
        private int _replicateMouseClicksDelay = 200;
        private string? _replicateMouseClicksKeyBinding;
        private bool _autoSwitchOnFightTurn;
        private bool _autoSwitchOnGroupInvite;
        private bool _autoSwitchOnTradeInvite;

        [JsonIgnore]
        public bool ReplicateMouseClicks
        {
            get => _replicateMouseClicks;
            set => SetProperty(ref _replicateMouseClicks, value);
        }

        public int ReplicateMouseClicksDelay
        {
            get => _replicateMouseClicksDelay;
            set => SetProperty(ref _replicateMouseClicksDelay, value);
        }

        public string? ReplicateMouseClicksKeyBinding
        {
            get => _replicateMouseClicksKeyBinding;
            set => SetProperty(ref _replicateMouseClicksKeyBinding, value);
        }

        public bool AutoSwitchOnFightTurn
        {
            get => _autoSwitchOnFightTurn;
            set => SetProperty(ref _autoSwitchOnFightTurn, value);
        }

        public bool AutoSwitchOnGroupInvite
        {
            get => _autoSwitchOnGroupInvite;
            set => SetProperty(ref _autoSwitchOnGroupInvite, value);
        }

        public bool AutoSwitchOnTradeInvite
        {
            get => _autoSwitchOnTradeInvite;
            set => SetProperty(ref _autoSwitchOnTradeInvite, value);
        }
    }
}
