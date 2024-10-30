using System.Configuration;

namespace TractorSupporter.Services;

public partial class AvoidingService: CommandService
{
    private readonly List<DateTime> _avoidingDistanceTimes;
    private double _avoidingDistance;
    private int _minAvoidingSignalsCount;
    private int _avoidingDistanceSignalValidLifetimeMs;
    private bool _avoidingDecisionAllowed;

    private AvoidingService()
    {
        _avoidingDistanceTimes = new List<DateTime>();
        _avoidingDistance = double.Parse(ConfigurationManager.AppSettings["AvoidingDistance"]!);
        _minAvoidingSignalsCount = int.Parse(ConfigurationManager.AppSettings["MinSignalsCount"]!);
        _avoidingDistanceSignalValidLifetimeMs = int.Parse(ConfigurationManager.AppSettings["SignalValidLifetimeMs"]!);
        _avoidingDecisionAllowed = false;
    }

    public void AllowMakingDecision(bool shouldAllowMakingDecision)
    {
        _avoidingDecisionAllowed = shouldAllowMakingDecision;
    }

    public bool MakeAvoidingDecision(double distanceMeasured)
    {
        return MakeDecision(
            distanceMeasured,
            _avoidingDistanceTimes,
            _avoidingDistance,
            _avoidingDistanceSignalValidLifetimeMs,
            _minAvoidingSignalsCount,
            ref _avoidingDecisionAllowed,
            true
        );
    }
}

#region Class structure 
public partial class AvoidingService
{
    private static readonly Lazy<AvoidingService> _lazyInstance = new(() => new AvoidingService());
    public static AvoidingService Instance => _lazyInstance.Value;
}
#endregion
