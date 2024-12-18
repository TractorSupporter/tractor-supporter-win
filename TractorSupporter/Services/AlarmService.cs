using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TractorSupporter.Model;
using TractorSupporter.Services.Abstract;

namespace TractorSupporter.Services;

public interface IAlarmService
{
    public double AlarmDistance { get; set; }
    public bool MakeAlarmDecision(double distanceMeasured);
    public void ChangeConfig(bool isLidar);
}

public partial class AlarmService: CommandFieldDecision, IAlarmService
{
    private readonly List<(DateTime, double)> _alarmDistanceTimes;
    public double AlarmDistance { get; set; }
    private int _minAlarmSignalsCount;
    private int _alarmDistanceSignalValidLifetimeMs;
    private bool _alarmDecisionAllowed;
    private ILoggingService _loggingService;

    public AlarmService(ILoggingService logging)
    {
        _loggingService = logging;
        _alarmDistanceTimes = new List<(DateTime, double)>();
        _minAlarmSignalsCount = int.Parse(ConfigurationManager.AppSettings["MinSignalsCount"]!);
        _alarmDistanceSignalValidLifetimeMs = int.Parse(ConfigurationManager.AppSettings["SignalValidLifetimeMs"]!);
    }

    public void ChangeConfig(bool isLidar)
    {
        if (isLidar)
            _minAlarmSignalsCount = int.Parse(ConfigurationManager.AppSettings["MinSignalsCountLidar"]!);
        else
            _minAlarmSignalsCount = int.Parse(ConfigurationManager.AppSettings["MinSignalsCount"]!);
    }

    public bool MakeAlarmDecision(double distanceMeasured)
    {
        var decision = MakeDecision(
            distanceMeasured,
            _alarmDistanceTimes,
            AlarmDistance,
            _alarmDistanceSignalValidLifetimeMs,
            _minAlarmSignalsCount
        );

        if (decision)
            _loggingService.AddLog(Model.Enums.DecisionType.Alarm);

        return decision;
    }
}

