using System.Windows;
using TractorSupporter.ViewModel;

namespace TractorSupporter
{
    public partial class TSWindow : Window
    {
        public TSWindow()
        {
            InitializeComponent();
            DataContext = new TSWindowViewModel();
        }
    }
}