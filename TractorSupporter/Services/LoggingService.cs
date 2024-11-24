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

public interface ILoggingService
{
    public ObservableCollection<LogEntry> Logs { get; set; }
    public void AddLog(DecisionType decisionType);
}

public partial class LoggingService: ILoggingService
{
    private ObservableCollection<LogEntry> _allLogs = new ObservableCollection<LogEntry>();

    public ObservableCollection<LogEntry> Logs { get; set; }

    public void AddLog(DecisionType decisionType)
    {
        App.Current.Dispatcher.Invoke(() => { Logs.Add(new LogEntry { Time = $"[{DateTime.Now.ToShortTimeString()}]", DecisionType = decisionType }); });
        
    }

    public LoggingService()
    {
        Logs = new ObservableCollection<LogEntry>();
    }
}
