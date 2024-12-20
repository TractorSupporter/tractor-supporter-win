﻿using Microsoft.Extensions.DependencyInjection;
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
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
            DataContext = new SettingsPageViewModel(App.ServiceProvider.GetRequiredService<IReceivedDataFormatter>());
        }

        private void SettingsOption_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
