using System.Windows;
using System.Windows.Controls;
using TractorSupporter.ViewModel;

namespace TractorSupporter.View;

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
