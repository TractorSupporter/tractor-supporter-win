﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TractorSupporter.Model;
using TractorSupporter.Services.Abstract;

namespace TractorSupporter.Services;

public partial class AlarmService: CommandFieldDecision
{
    private readonly List<DateTime> _alarmDistanceTimes;
    public double AlarmDistance { get; set; }
    private int _minAlarmSignalsCount;
    private int _alarmDistanceSignalValidLifetimeMs;
    private bool _alarmDecisionAllowed;

    private AlarmService()
    {
        _alarmDistanceTimes = new List<DateTime>();
        _minAlarmSignalsCount = int.Parse(ConfigurationManager.AppSettings["MinSignalsCount"]!);
        _alarmDistanceSignalValidLifetimeMs = int.Parse(ConfigurationManager.AppSettings["SignalValidLifetimeMs"]!);
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
            LoggingService.Instance.AddLog(Model.Enums.DecisionType.Alarm);

        return decision;
    }
}

#region Class structure 
public partial class AlarmService
{
    private static readonly Lazy<AlarmService> _lazyInstance = new(() => new AlarmService());
    public static AlarmService Instance => _lazyInstance.Value;
}
#endregion
