using System.Collections.ObjectModel;
using System.Windows.Controls;
using TractorSupporter.Model;
using TractorSupporter.Model.Enums;
using TractorSupporter.Services;

namespace TractorSupporter.View.Controls;



/// <summary>
/// Interaction logic for LogViewer.xaml
/// </summary>
public partial class LogViewer : UserControl
{
    
    public ObservableCollection<LogEntry> Logs
    {
        get => LoggingService.Instance.Logs;
    }

    public LogViewer()
    {
        InitializeComponent();
        DataContext = this;

        
    }

    private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (LogsListBox.Items.Count > 0)
        {
            LogsListBox.ScrollIntoView(LogsListBox.Items[LogsListBox.Items.Count - 1]);
        }
    }
}
