﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TractorSupporter.Services;
using TractorSupporter.Services.Interfaces;
using TractorSupporter.ViewModel;

namespace TractorSupporter.View.Controls
{
    public partial class Navbar : UserControl
    {
        public Action OnSettingsClicked { get; set; }


        public Navbar()
        {
            InitializeComponent();
        }

        public ICommand SettingsCommand { get; }

        public bool IsSettingsVisible
        {
            get { return (bool)GetValue(IsSettingsVisibleProperty); }
            set { SetValue(IsSettingsVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsSettingsVisibleProperty =
            DependencyProperty.Register("IsSettingsVisible", typeof(bool), typeof(Navbar), new PropertyMetadata(false));

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            OnSettingsClicked?.Invoke();
        }
    }
}
