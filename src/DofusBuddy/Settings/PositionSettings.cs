using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DofusBuddy.Settings
{
    public class PositionSettings : ObservableObject
    {
        private Point _skipTurnButtonPosition;
        private Point _acceptButtonPosition;

        public Point SkipTurnButtonPosition
        {
            get => _skipTurnButtonPosition;
            set => SetProperty(ref _skipTurnButtonPosition, value);
        }

        public Point AcceptButtonPosition
        {
            get => _acceptButtonPosition;
            set => SetProperty(ref _acceptButtonPosition, value);
        }
    }
}
