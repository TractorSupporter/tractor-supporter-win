using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TractorSupporter.Services;

public class CommandFieldDecisionService
{
    public bool MakeDecision(double distanceMeasured, List<DateTime> distanceTimes, double distance, int validLifetimeMs, int minSignalsCount)
    {
        var currentTime = DateTime.Now;

        distanceTimes.RemoveAll(time => (currentTime - time).TotalMilliseconds > validLifetimeMs);

        if (distanceMeasured <= distance)
            distanceTimes.Add(currentTime);

        var decision = distanceTimes.Count >= minSignalsCount;

        return decision;
    }
}
