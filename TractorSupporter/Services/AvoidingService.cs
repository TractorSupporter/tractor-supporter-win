using System.Configuration;
using TractorSupporter.Model;
using TractorSupporter.Services.Abstract;

namespace TractorSupporter.Services;

public partial class AvoidingService: CommandFieldDecision
{
    private readonly List<DateTime> _avoidingDistanceTimes;
    public double AvoidingDistance { get; set; }
    private int _minAvoidingSignalsCount;
    private int _avoidingDistanceSignalValidLifetimeMs;
    private bool _avoidingDecisionAllowed;

    private AvoidingService()
    {
        DataReceiverGPS.Instance.ReceivedAllowMakingDecision += AllowMakingDecision;
        _avoidingDistanceTimes = new List<DateTime>();
        _minAvoidingSignalsCount = int.Parse(ConfigurationManager.AppSettings["MinSignalsCount"]!);
        _avoidingDistanceSignalValidLifetimeMs = int.Parse(ConfigurationManager.AppSettings["SignalValidLifetimeMs"]!);
        _avoidingDecisionAllowed = false;
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

        if (!_avoidingDecisionAllowed)
            return false;

        if (decision)
            _avoidingDecisionAllowed = false;

        if (decision)
            LoggingService.Instance.AddLog(Model.Enums.DecisionType.Avoid);

        return decision;
    }
}

#region Class structure 
public partial class AvoidingService
{
    // not lazy, i want to register event hander at the begining of the app execution
    private static readonly AvoidingService _instance = new AvoidingService();
    public static AvoidingService Instance => _instance;
}
#endregion
