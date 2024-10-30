using System.Configuration;
using TractorSupporter.Model;

namespace TractorSupporter.Services;

public partial class AvoidingService: CommandFieldDecisionService
{
    private readonly List<DateTime> _avoidingDistanceTimes;
    public double AvoidingDistance { get; set; }
    private int _minAvoidingSignalsCount;
    private int _avoidingDistanceSignalValidLifetimeMs;
    private bool _avoidingDecisionAllowed;

    private AvoidingService()
    {
        _avoidingDistanceTimes = new List<DateTime>();
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

        return decision;
    }
}

#region Class structure 
public partial class AvoidingService
{
    private static readonly Lazy<AvoidingService> _lazyInstance = new(() => new AvoidingService());
    public static AvoidingService Instance => _lazyInstance.Value;
}
#endregion
