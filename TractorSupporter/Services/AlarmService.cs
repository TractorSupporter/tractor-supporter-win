using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TractorSupporter.Services;

public partial class AlarmService: CommandService
{
    private readonly List<DateTime> _alarmDistanceTimes;
    private double _alarmDistance;
    private int _minAlarmSignalsCount;
    private int _alarmDistanceSignalValidLifetimeMs;
    private bool _alarmDecisionAllowed;

    private AlarmService()
    {
        _alarmDistanceTimes = new List<DateTime>();
        _alarmDistance = double.Parse(ConfigurationManager.AppSettings["AlarmDistance"]!);
        _minAlarmSignalsCount = int.Parse(ConfigurationManager.AppSettings["MinSignalsCount"]!);
        _alarmDistanceSignalValidLifetimeMs = int.Parse(ConfigurationManager.AppSettings["SignalValidLifetimeMs"]!);
        _alarmDecisionAllowed = true;
    }

    public void AllowMakingDecision(bool shouldAllowMakingDecision)
    {
        _alarmDecisionAllowed = shouldAllowMakingDecision;
    }

    public bool MakeAlarmDecision(double distanceMeasured)
    {
        return MakeDecision(
            distanceMeasured,
            _alarmDistanceTimes,
            _alarmDistance,
            _alarmDistanceSignalValidLifetimeMs,
            _minAlarmSignalsCount,
            ref _alarmDecisionAllowed
        );
    }
}

#region Class structure 
public partial class AlarmService
{
    private static readonly Lazy<AlarmService> _lazyInstance = new(() => new AlarmService());
    public static AlarmService Instance => _lazyInstance.Value;
}
#endregion
