using System.Configuration;

namespace TractorSupporter.Services.Abstract;

public abstract class CommandFieldDecision
{
    private readonly int _minDistance = int.Parse(ConfigurationManager.AppSettings["MinDistance"] ?? "0");

    public bool MakeDecision(double distanceMeasured, List<(DateTime time, double dist)> distanceTimes, double distance, int validLifetimeMs, int minSignalsCount)
    {
        var currentTime = DateTime.Now;

        distanceTimes.RemoveAll(tim => (currentTime - tim.time).TotalMilliseconds > validLifetimeMs);

        if (distanceMeasured <= distance && distance >= _minDistance)
            distanceTimes.Add((currentTime, distanceMeasured));



        var decision = distanceTimes.Count >= minSignalsCount;
        if (decision)
        {
            var max = distanceTimes.Select(x => x.dist).Max();
            var min = distanceTimes.Select(x => x.dist).Min();

            var valid = max - min < 100;

            return valid;
        }
        else return false;
    }
}
