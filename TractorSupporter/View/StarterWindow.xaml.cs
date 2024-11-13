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
            Navbar.OnSettingsClicked = viewModel.CloseMainWindow;

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
                    case SettingsPage:
                        viewModel.IsSettingsVisible = true;
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
