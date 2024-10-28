using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TractorSupporter.Services;

public class CommandService
{
    public bool MakeDecision(double distanceMeasured, List<DateTime> distanceTimes, double distance, int validLifetimeMs, int minSignalsCount, ref bool decisionAllowed, bool isAvoidingCommand = false)
    {
        var currentTime = DateTime.Now;

        distanceTimes.RemoveAll(time => (currentTime - time).TotalMilliseconds > validLifetimeMs);

        if (distanceMeasured <= distance)
            distanceTimes.Add(currentTime);

        var decision = distanceTimes.Count >= minSignalsCount;

        if (!decisionAllowed)
            return false;

        if (isAvoidingCommand && decision)
            decisionAllowed = false;

        return decision;
    }
}
