using System.Collections.ObjectModel;
using System.Windows.Controls;
using TractorSupporter.Model.Enums;

namespace TractorSupporter.View.Controls;

public class LogEntry
{
    public string Time { get; set; }
    public DecisionType DecisionType { get; set; }
}

/// <summary>
/// Interaction logic for LogViewer.xaml
/// </summary>
public partial class LogViewer : UserControl
{
    private Collection<LogEntry> _allLogs = new Collection<LogEntry>();

    public Collection<LogEntry> Logs
    {
        get => _allLogs;
        set => _allLogs = value;
    }

    public LogViewer()
    {
        InitializeComponent();
        DataContext = this;

        Logs = new Collection<LogEntry>
        {
            new LogEntry { Time = "[12:00]", DecisionType = DecisionType.Avoid },
            new LogEntry { Time = "[12:30]", DecisionType = DecisionType.Alarm },
            new LogEntry { Time = "[13:00]", DecisionType = DecisionType.Avoid },
        };
    }

    public void AddLog(TimeOnly time, DecisionType decisionType)
    {
        _allLogs.Add(new LogEntry { Time = $"[{time.ToShortTimeString()}]", DecisionType = decisionType });
    }
}
