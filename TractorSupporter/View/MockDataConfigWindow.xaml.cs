using System.Windows;
using TractorSupporter.ViewModel;

namespace TractorSupporter;

public partial class MockDataConfigWindow : Window
{
    private IMockDataConfigWindowViewModel mockDataConfigWindowViewModel;
    public MockDataConfigWindow(IMockDataConfigWindowViewModel mockdataconfigwvm)
    {
        InitializeComponent();
        mockDataConfigWindowViewModel = mockdataconfigwvm;
        DataContext = mockDataConfigWindowViewModel;
    }
}