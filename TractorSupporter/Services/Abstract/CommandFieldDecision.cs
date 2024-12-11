using System.Configuration;

namespace TractorSupporter.Services.Abstract;

public abstract class CommandFieldDecision
{
    private readonly int _minDistance = int.Parse(ConfigurationManager.AppSettings["MinDistance"] ?? "0");

    public bool MakeDecision(double distanceMeasured, List<DateTime> distanceTimes, double distance, int validLifetimeMs, int minSignalsCount)
    {
        var currentTime = DateTime.Now;

        distanceTimes.RemoveAll(time => (currentTime - time).TotalMilliseconds > validLifetimeMs);

        if (distanceMeasured <= distance && distance >= _minDistance)
            distanceTimes.Add(currentTime);

        var decision = distanceTimes.Count >= minSignalsCount;

        return decision;
    }
}
