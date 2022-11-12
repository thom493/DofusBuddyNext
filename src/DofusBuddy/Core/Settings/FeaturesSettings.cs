using CommunityToolkit.Mvvm.ComponentModel;

namespace DofusBuddy.Core.Settings
{
    public class FeaturesSettings : ObservableObject
    {
        private bool _replicateMouseClicks;
        private string? _replicateMouseClicksKeyBinding;
        private bool _autoSwitchInFight;
        private bool _autoSwitchOnGroupInvite;
        private bool _autoSwitchOnTradeInvite;

        public bool ReplicateMouseClicks
        {
            get => _replicateMouseClicks;
            set =>
                SetProperty(ref _replicateMouseClicks, value);
        }

        public string? ReplicateMouseClicksKeyBinding
        {
            get => _replicateMouseClicksKeyBinding;
            set =>
                SetProperty(ref _replicateMouseClicksKeyBinding, value);
        }

        public bool AutoSwitchInFight
        {
            get => _autoSwitchInFight;
            set =>
                SetProperty(ref _autoSwitchInFight, value);
        }

        public bool AutoSwitchOnGroupInvite
        {
            get => _autoSwitchOnGroupInvite;
            set =>
                SetProperty(ref _autoSwitchOnGroupInvite, value);
        }

        public bool AutoSwitchOnTradeInvite
        {
            get => _autoSwitchOnTradeInvite;
            set =>
                SetProperty(ref _autoSwitchOnTradeInvite, value);
        }
    }
}
