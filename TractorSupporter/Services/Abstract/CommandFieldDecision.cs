using System.Configuration;
using TractorSupporter.Model;

namespace TractorSupporter.Services.Abstract;

public abstract class CommandFieldDecision
{
    private readonly int _minDistance = int.Parse(ConfigurationManager.AppSettings["MinDistance"] ?? "0");
    public Dictionary<int, CircularBuffer<double>> measurements;

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

    public bool MakeDecision(Dictionary<int, double> newMeasurements, double speed, List<(DateTime time, double dist)> distanceTimes, double distance, int validLifetimeMs, int minSignalsCount)
    {
        foreach (var (angle, measurement) in newMeasurements)
        {
            measurements[angle].Add(measurement);
        }

        return false;
    }

    public void InitMeasurements()
    {
        measurements = new Dictionary<int, CircularBuffer<double>>();

        for (int i = -30; i < 30; i++)
        {
            measurements.Add(i, new CircularBuffer<double>(6));
        }
    }
}
