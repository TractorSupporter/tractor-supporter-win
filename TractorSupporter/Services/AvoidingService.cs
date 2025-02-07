﻿using System.Configuration;
using TractorSupporter.Model;
using TractorSupporter.Services.Abstract;

namespace TractorSupporter.Services;

public interface IAvoidingService
{
    public double AvoidingDistance { get; set; }
    public bool MakeAvoidingDecision(double distanceMeasured);
    public bool MakeLidarAvoidingDecision(double distanceMeasured);
    public void AllowMakingDecision(object? sender, bool shouldAllowMakingDecision);
    public void ChangeConfig(bool isLidar);
}

public partial class AvoidingService: CommandFieldDecision, IAvoidingService
{
    private ILoggingService _logging;
    private readonly List<(DateTime, double)> _avoidingDistanceTimes;
    public double AvoidingDistance { get; set; }
    private int _minAvoidingSignalsCount;
    private int _avoidingDistanceSignalValidLifetimeMs;
    private bool _avoidingDecisionAllowed;
    private IDataSenderGPS _senderGPS;
    private IDataReceiverGPS _receiverGPS;

    public AvoidingService(ILoggingService loggingService, IGPSConnectionService gpsConnection, IDataReceiverGPS dataReceiverGPS, IDataSenderGPS dataSenderGPS)
    {
        obstacles = new List<Obstacle2D>();
        _receiverGPS = dataReceiverGPS;
        _senderGPS = dataSenderGPS;
        _logging = loggingService;
        gpsConnection.ConnectedToGPSUpdated += OnConnectionToGPSChanged;
        _receiverGPS.ReceivedAvoidingDecisionState += AllowMakingDecision;
        _avoidingDistanceTimes = new List<(DateTime, double)>();
        _minAvoidingSignalsCount = int.Parse(ConfigurationManager.AppSettings["MinSignalsCount"] ?? "0");
        _avoidingDistanceSignalValidLifetimeMs = int.Parse(ConfigurationManager.AppSettings["SignalValidLifetimeMs"] ?? "0");
        _avoidingDecisionAllowed = false;
    }


    public void ChangeConfig(bool isLidar)
    {
        if (isLidar)
            _minAvoidingSignalsCount = int.Parse(ConfigurationManager.AppSettings["MinSignalsCountLidar"] ?? "0");
        else
            _minAvoidingSignalsCount = int.Parse(ConfigurationManager.AppSettings["MinSignalsCount"] ?? "0");
    }

    public async void OnConnectionToGPSChanged(object? sender, bool isConnected)
    {
        if (isConnected)
            await _senderGPS.SendData(new { askForApplicationState = true });


    }

    public void AllowMakingDecision(object? sender, bool shouldAllowMakingDecision)
    {
        _avoidingDecisionAllowed = shouldAllowMakingDecision;
    }

    public bool MakeAvoidingDecision(double distanceMeasured)
    {
        bool decision = MakeDecision(
            distanceMeasured,
            _avoidingDistanceTimes,
            AvoidingDistance,
            _avoidingDistanceSignalValidLifetimeMs,
            _minAvoidingSignalsCount
        );

        decision = processDecision(decision);

        return decision;
    }

    public bool MakeLidarAvoidingDecision(double distanceMeasured)
    {
        bool decision = MakeDecision(
            distanceMeasured,
            AvoidingDistance
        );

        decision = processDecision(decision);

        return decision;
    }

    private bool processDecision(bool decision)
    {
        if (!_avoidingDecisionAllowed)
            return false;

        if (decision)
            _avoidingDecisionAllowed = false;

        if (decision)
            _logging.AddLog(Model.Enums.DecisionType.Avoid);

        return decision;
    }
}
