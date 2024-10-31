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
using TractorSupporter.ViewModel;

namespace TractorSupporter.View
{
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            DataContext = new MainPageViewModel();
            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.IsInitialized && App.CommandLineArgs != null && App.CommandLineArgs.Length > 0 && App.CommandLineArgs[0] == "click_connect_button")
            {
                if (DataContext is MainPageViewModel viewModel)
                {
                    if (viewModel.StartConnectionCommand.CanExecute(null))
                    {
                        viewModel.StartConnectionCommand.Execute(null);
                    }
                }
            }
            App.IsInitialized = false;
        }
    }
}
