using System.Configuration;
using TractorSupporter.Model;
using TractorSupporter.Services.Abstract;

namespace TractorSupporter.Services;

public interface IAvoidingService
{
    public double AvoidingDistance { get; set; }
    public bool MakeAvoidingDecision(double distanceMeasured);
    public void AllowMakingDecision(object? sender, bool shouldAllowMakingDecision);
}

public partial class AvoidingService: CommandFieldDecision, IAvoidingService
{
    private ILoggingService _logging;
    private readonly List<DateTime> _avoidingDistanceTimes;
    public double AvoidingDistance { get; set; }
    private int _minAvoidingSignalsCount;
    private int _avoidingDistanceSignalValidLifetimeMs;
    private bool _avoidingDecisionAllowed;

    public AvoidingService(ILoggingService loggingService)
    {
        _logging = loggingService;
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
            _logging.AddLog(Model.Enums.DecisionType.Avoid);

        return decision;
    }
}
