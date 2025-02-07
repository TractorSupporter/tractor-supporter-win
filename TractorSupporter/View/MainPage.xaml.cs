﻿using System.Windows;
using System.Windows.Controls;
using TractorSupporter.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

namespace TractorSupporter.View;

public partial class MainPage : Page
{
    public MainPage()
    {
        InitializeComponent();
        var viewModel = App.ServiceProvider.GetRequiredService<MainPageViewModel>();
        DataContext = viewModel;
        this.Loaded += MainPage_Loaded;
    }

    private async void MainPage_Loaded(object sender, RoutedEventArgs e)
    {
        if (App.IsInitialized && App.CommandLineArgs != null && App.CommandLineArgs.Length > 0 && App.CommandLineArgs[0] == "click_connect_button")
        {
            if (DataContext is MainPageViewModel viewModel)
            {
                if (viewModel.StartConnectionCommand.CanExecute(null))
                {
                    await Task.Delay(500);
                    viewModel.StartConnectionCommand.Execute(null);
                }
            }
        }
        App.IsInitialized = false;
    }
}
