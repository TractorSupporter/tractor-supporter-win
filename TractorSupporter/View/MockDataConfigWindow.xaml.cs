using System.Windows;
using TractorSupporter.ViewModel;

namespace TractorSupporter
{
    public partial class MockDataConfigWindow : Window
    {
        public MockDataConfigWindow()
        {
            InitializeComponent();
            DataContext = new MockDataConfigWindowViewModel();
        }
    }
}