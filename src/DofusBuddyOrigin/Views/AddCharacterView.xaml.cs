using System;
using DofusBuddy.ViewModels;
using Wpf.Ui.Controls;

namespace DofusBuddy.Views
{
    /// <summary>
    /// Interaction logic for AddCharacterView.xaml
    /// </summary>
    public partial class AddCharacterView : UiWindow
    {
        private readonly AddCharacterViewModel _addCharacterViewModel;

        public AddCharacterView(AddCharacterViewModel characterDetectionViewModel)
        {
            InitializeComponent();
            _addCharacterViewModel = characterDetectionViewModel;
            DataContext = characterDetectionViewModel;
            characterDetectionViewModel.Initialize();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _addCharacterViewModel.Dispose();
        }
    }
}
