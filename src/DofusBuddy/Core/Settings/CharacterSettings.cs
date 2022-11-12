using CommunityToolkit.Mvvm.ComponentModel;

namespace DofusBuddy.Core.Settings
{
    public class CharacterSettings : ObservableObject
    {
        private string? _id;
        private string? _name;
        private int _order;
        private bool _follow;
        private bool _autoSkipInFight;
        private string? _focusWindowKeyBinding;

        public string? Id
        {
            get => _id;
            set
            {
                SetProperty(ref _id, value);
            }
        }

        public string? Name
        {
            get => _name;
            set
            {
                SetProperty(ref _name, value);
            }
        }

        public int Order
        {
            get => _order; set
            {

                SetProperty(ref _order, value);
            }
        }

        public bool Follow
        {
            get => _follow; set
            {
                SetProperty(ref _follow, value);
            }
        }

        public bool AutoSkipInFight
        {
            get => _autoSkipInFight; set
            {
                SetProperty(ref _autoSkipInFight, value);
            }
        }

        public string? FocusWindowKeyBinding
        {
            get => _focusWindowKeyBinding;
            set
            {
                SetProperty(ref _focusWindowKeyBinding, value);
            }
        }
    }
}
