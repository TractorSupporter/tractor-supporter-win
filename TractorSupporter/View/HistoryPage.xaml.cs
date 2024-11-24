using System.Windows;
using System.Windows.Controls;
using TractorSupporter.View.Controls;
using TractorSupporter.ViewModel;

namespace TractorSupporter.View;

/// <summary>
/// Interaction logic for HistoryPage.xaml
/// </summary>
public partial class HistoryPage : Page
{
    public HistoryPage()
    {
        InitializeComponent();
        DataContext = new HistoryPageViewModel();
    }
}
