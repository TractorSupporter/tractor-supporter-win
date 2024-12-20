﻿using TractorSupporter.Services;
using TractorSupporter.ViewModel;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace TractorSupporter.View
{
    public partial class TSContainerWindow : Window
    {
        public TSContainerWindow()
        {
            InitializeComponent();
            NavigationService.Initialize(MainFrame);
            DataContext = new TSContainerWindowViewModel(App.ServiceProvider.GetRequiredService<IReceivedDataFormatter>());
            Navbar.OnSettingsClicked = NavigationService.Instance.NavigateToSettings;
            Navbar.OnHistoryClicked = NavigationService.Instance.NavigateToHistory;
        }

        private void MainFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (DataContext is TSContainerWindowViewModel viewModel)
            {
                switch (e.Content)
                {
                    case StarterPage:
                        viewModel.IsHistoryVisible = false;
                        viewModel.IsSettingsVisible = false;
                        break;
                    case SettingsPage:
                        viewModel.IsHistoryVisible = true;
                        viewModel.IsSettingsVisible = false;
                        break;
                    case MainPage:
                        viewModel.IsHistoryVisible = true;
                        viewModel.IsSettingsVisible = true;
                        break;
                    case HistoryPage:
                        viewModel.IsHistoryVisible = false;
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
