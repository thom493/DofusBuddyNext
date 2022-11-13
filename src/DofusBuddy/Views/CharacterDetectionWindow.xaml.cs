using System;
using DofusBuddy.ViewModels;
using Wpf.Ui.Controls;

namespace DofusBuddy.Views
{
    /// <summary>
    /// Interaction logic for CharacterDetectionWindow.xaml
    /// </summary>
    public partial class CharacterDetectionWindow : UiWindow
    {
        private readonly CharacterDetectionViewModel _characterDetectionViewModel;

        public CharacterDetectionWindow(CharacterDetectionViewModel characterDetectionViewModel)
        {
            InitializeComponent();
            _characterDetectionViewModel = characterDetectionViewModel;
            DataContext = characterDetectionViewModel;
            characterDetectionViewModel.Initialize();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _characterDetectionViewModel.Dispose();
        }
    }
}
