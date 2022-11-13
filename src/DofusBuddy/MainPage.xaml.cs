using System.Windows.Controls;
using DofusBuddy.ViewModels;

namespace DofusBuddy
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {

        public MainPage(MainPageViewModel mainPageViewModel)
        {
            InitializeComponent();
            DataContext = mainPageViewModel;
            mainPageViewModel.Initialize();
        }
    }
}
