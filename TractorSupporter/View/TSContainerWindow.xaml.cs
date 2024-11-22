using TractorSupporter.Services;
using TractorSupporter.ViewModel;
using System.Windows;

namespace TractorSupporter.View
{
    public partial class TSContainerWindow : Window
    {
        public TSContainerWindow()
        {
            InitializeComponent();
            NavigationService.Initialize(MainFrame);
            DataContext = new TSContainerWindowViewModel();
            Navbar.OnSettingsClicked = NavigationService.Instance.NavigateToSettings;
        }

        private void MainFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (DataContext is TSContainerWindowViewModel viewModel)
            {
                switch (e.Content)
                {
                    case StarterPage:
                        viewModel.IsSettingsVisible = false;
                        break;
                    case SettingsPage:
                        viewModel.IsSettingsVisible = false;
                        break;
                    case MainPage:
                        viewModel.IsSettingsVisible = true;
                        break;
                    default:
                        viewModel.IsSettingsVisible = true;
                        break;
                }
            }
        }
    }
}
