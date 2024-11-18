using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TractorSupporter.Services;
using TractorSupporter.ViewModel;

namespace TractorSupporter.View
{
    public partial class StarterWindow : Window
    {
        public StarterWindow()
        {
            InitializeComponent();
            NavigationService.Initialize(MainFrame);
            DataContext = new StarterWindowViewModel();

            var viewModel = DataContext as StarterWindowViewModel;
            Navbar.OnSettingsClicked = viewModel.SettingsCloseMainWindow;
            Navbar.OnHistoryClicked = viewModel.HistoryCloseMainWindow;

            if (App.CommandLineArgs != null && ConfigAppJson.Instance.ReadJson() == null)
            {
                App.IsInitialized = false;
            }
        }

        private void MainFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (DataContext is StarterWindowViewModel viewModel)
            {
                switch (e.Content)
                {
                    case HistoryPage:
                        viewModel.IsHistoryVisible = true;
                        viewModel.IsSettingsVisible = true;
                        break;

                    case SettingsPage:
                        viewModel.IsHistoryVisible = true;
                        viewModel.IsSettingsVisible = true;
                        break;

                    case MainPage:
                        viewModel.IsHistoryVisible = true;
                        viewModel.IsSettingsVisible = true;
                        break;

                    default:
                        viewModel.IsHistoryVisible = true;
                        viewModel.IsSettingsVisible = true;
                        break;
                }
            }
        }
    }
}
