namespace TractorSupporter.Model;

public static class AvoidingLogic
{
    public static bool isAvoidingCommandLocked = false;
    public static bool isSendingAvoidCommandTurnedOn = false;

    public static bool makeAvoidingDecision(bool avoidSignal, ref bool isAvoidingCommandLocked)
    {
        // todo more complex logic, take into consideration signals from last 650 ms for ex.
        // also block sending positive decision when tractor is executing a turn.
        // wait for AgOpenGPS signal from some kind of pipe to unlock positive decisions.
        if (isAvoidingCommandLocked == true)
            return false;
        isAvoidingCommandLocked = true;
        return avoidSignal;
    }
}
