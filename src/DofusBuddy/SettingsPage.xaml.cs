namespace DofusBuddy
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private void TaskbarStateComboBox_OnSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (sender is not System.Windows.Controls.ComboBox comboBox)
            {
                return;
            }

            System.Windows.Window parentWindow = System.Windows.Window.GetWindow(this);

            if (parentWindow == null)
            {
                return;
            }

            int selectedIndex = comboBox.SelectedIndex;

            switch (selectedIndex)
            {
                case 1:
                    Wpf.Ui.TaskBar.TaskBarProgress.SetValue(
                        parentWindow,
                        Wpf.Ui.TaskBar.TaskBarProgressState.Normal,
                        80);
                    break;

                case 2:
                    Wpf.Ui.TaskBar.TaskBarProgress.SetValue(
                        parentWindow,
                        Wpf.Ui.TaskBar.TaskBarProgressState.Error,
                        80);
                    break;

                case 3:
                    Wpf.Ui.TaskBar.TaskBarProgress.SetValue(
                        parentWindow,
                        Wpf.Ui.TaskBar.TaskBarProgressState.Paused,
                        80);
                    break;

                case 4:
                    Wpf.Ui.TaskBar.TaskBarProgress.SetValue(
                        parentWindow,
                        Wpf.Ui.TaskBar.TaskBarProgressState.Indeterminate,
                        80);
                    break;

                default:
                    Wpf.Ui.TaskBar.TaskBarProgress.SetState(parentWindow, Wpf.Ui.TaskBar.TaskBarProgressState.None);
                    break;
            }
        }
    }
}
