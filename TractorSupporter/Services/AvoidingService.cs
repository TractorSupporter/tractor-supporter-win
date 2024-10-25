using System.Configuration;

namespace TractorSupporter.Services;



public partial class AvoidingService
{
    private readonly List<DateTime> _avoidingDistanceTimes;
    private double _avoidingDistance;
    private double _minAvoidingSignalsCount;
    private int _avoidingDistanceSignalValidLifetimeMs;
    private bool _avoidingDecisionAllowed;

    private AvoidingService()
    {
        _avoidingDistanceTimes = new List<DateTime>();
        _avoidingDistance = double.Parse(ConfigurationManager.AppSettings["AvoidingDistance"]!);
        _minAvoidingSignalsCount = int.Parse(ConfigurationManager.AppSettings["MinAvoidingSignalsCount"]!);
        _avoidingDistanceSignalValidLifetimeMs = int.Parse(ConfigurationManager.AppSettings["AvoidingDistanceSignalValidLifetimeMs"]!);
        _avoidingDecisionAllowed = false;
    }

    public void AllowMakingDecision(bool shouldAllowMakingDecision)
    {
        _avoidingDecisionAllowed = shouldAllowMakingDecision;
    }

    public bool MakeAvoidingDecision(double distanceMeasured)
    {
        

        var currentTime = DateTime.Now;

        _avoidingDistanceTimes.RemoveAll(time => (currentTime - time).TotalMilliseconds > _avoidingDistanceSignalValidLifetimeMs);

        if (distanceMeasured <= _avoidingDistance)
            _avoidingDistanceTimes.Add(currentTime);

        var decision = _avoidingDistanceTimes.Count >= _minAvoidingSignalsCount;

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
