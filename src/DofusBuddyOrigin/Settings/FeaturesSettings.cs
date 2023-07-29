using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DofusBuddy.Settings
{
    public class FeaturesSettings : ObservableObject
    {
        private bool _replicateLeftMouseClicks;
        private int _replicateLeftMouseClicksDelay = 150;
        private int _precastSpellAwaitDelay = 1000;
        private int _cooldownTurn = 5;
        private string? _replicateMouseClicksKeyBinding;
        private bool _leftMouseClickOnWheelClick;
        private bool _autoSwitchOnFightTurn;
        private bool _precastFirstSpell;
        private bool _autoAcceptGroupInvitation;
        private bool _autoAcceptTradeInvitation;

        [JsonIgnore]
        public bool ReplicateLeftMouseClicks
        {
            get => _replicateLeftMouseClicks;
            set => SetProperty(ref _replicateLeftMouseClicks, value);
        }

        public int ReplicateLeftMouseClicksDelay
        {
            get => _replicateLeftMouseClicksDelay;
            set => SetProperty(ref _replicateLeftMouseClicksDelay, value);
        }

        public int PrecastSpellAwaitDelay
        {
            get => _precastSpellAwaitDelay;
            set => SetProperty(ref _precastSpellAwaitDelay, value);
        }

        public int CooldownTurn
        {
            get => _cooldownTurn;
            set => SetProperty(ref _cooldownTurn, value);
        }

        public string? ReplicateMouseClicksKeyBinding
        {
            get => _replicateMouseClicksKeyBinding;
            set => SetProperty(ref _replicateMouseClicksKeyBinding, value);
        }

        public bool LeftMouseClickOnWheelClick
        {
            get => _leftMouseClickOnWheelClick;
            set => SetProperty(ref _leftMouseClickOnWheelClick, value);
        }

        public bool AutoSwitchOnFightTurn
        {
            get => _autoSwitchOnFightTurn;
            set => SetProperty(ref _autoSwitchOnFightTurn, value);
        }

        public bool PrecastFirstSpell
        {
            get => _precastFirstSpell;
            set => SetProperty(ref _precastFirstSpell, value);
        }

        public bool AutoAcceptGroupInvitation
        {
            get => _autoAcceptGroupInvitation;
            set => SetProperty(ref _autoAcceptGroupInvitation, value);
        }

        public bool AutoAcceptTradeInvitation
        {
            get => _autoAcceptTradeInvitation;
            set => SetProperty(ref _autoAcceptTradeInvitation, value);
        }
    }
}
