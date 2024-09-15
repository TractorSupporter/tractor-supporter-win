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
    public partial class StarterConfigWindow : Window
    {
        public StarterConfigWindow()
        {
            InitializeComponent();
            DataContext = new StarterConfigViewModel();

            var viewModel = DataContext as StarterConfigViewModel;
            if (viewModel != null)
            {
                viewModel.RequestClose += (s, e) => this.Close();
            }
        }
    }
}
