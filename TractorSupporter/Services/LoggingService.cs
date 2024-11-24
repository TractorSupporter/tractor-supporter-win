using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TractorSupporter.Model;
using TractorSupporter.Model.Enums;
using TractorSupporter.View.Controls;

namespace TractorSupporter.Services;

public partial class LoggingService
{
    private ObservableCollection<LogEntry> _allLogs = new ObservableCollection<LogEntry>();

    public ObservableCollection<LogEntry> Logs { get; private set; }

    public void AddLog(DecisionType decisionType)
    {
        App.Current.Dispatcher.Invoke(() => { Logs.Add(new LogEntry { Time = $"[{DateTime.Now.ToShortTimeString()}]", DecisionType = decisionType }); });
        
    }

    private LoggingService()
    {
        Logs = new ObservableCollection<LogEntry>();
    }
}

#region Class structure 
public partial class LoggingService
{
    private static LoggingService _instance = new LoggingService();

    public static LoggingService Instance => _instance;
}
#endregion
