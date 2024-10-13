using System.Configuration;

namespace TractorSupporter.Services;



public partial class AvoidingService
{
    private readonly List<DateTime> _avoidingDistanceTimes;
    private double _avoidingDistance;
    private double _minAvoidingSignalsCount;
    private int _avoidingDistanceSignalValidLifetimeMs;
    private event Action UnblockMakingDecisionEvent;

    public void UnblockMakingDecision()
    {
        UnblockMakingDecisionEvent?.Invoke();
    }

    public bool MakeAvoidingDecision(double distanceMeasured)
    {
        var currentTime = DateTime.Now;

        _avoidingDistanceTimes.RemoveAll(time => (currentTime - time).TotalMilliseconds > _avoidingDistanceSignalValidLifetimeMs);

        if (distanceMeasured <= _avoidingDistance)
            _avoidingDistanceTimes.Add(currentTime);

        return _avoidingDistanceTimes.Count >= _minAvoidingSignalsCount;
    }
}

#region Class structure 
public partial class AvoidingService
{
    private static readonly Lazy<AvoidingService> _lazyInstance = new(() => new AvoidingService());
    public static AvoidingService Instance => _lazyInstance.Value;
    private AvoidingService() 
    {
        _avoidingDistanceTimes = new List<DateTime>();
        _avoidingDistance = double.Parse(ConfigurationManager.AppSettings["AvoidingDistance"]!);
        _minAvoidingSignalsCount = int.Parse(ConfigurationManager.AppSettings["MinAvoidingSignalsCount"]!);
        _avoidingDistanceSignalValidLifetimeMs = int.Parse(ConfigurationManager.AppSettings["AvoidingDistanceSignalValidLifetimeMs"]!);
    }
}
#endregion
