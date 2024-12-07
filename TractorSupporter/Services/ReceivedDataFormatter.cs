using System.Text.Json;

namespace TractorSupporter.Services;

public interface IReceivedDataFormatter : IInnerReceivedDataFormatter
{
    public void ChangeMode(bool isLidar);
}

public interface IInnerReceivedDataFormatter
{
    public (string extraMessage, double distanceMeasured) Format(JsonDocument data);
}

public class ReceivedDataFormatter : IReceivedDataFormatter
{
    private IInnerReceivedDataFormatter innerFormatter = new UltrasoundReceivedDataFormatter();

    public void ChangeMode(bool isLidar)
    {
        if (isLidar) innerFormatter = new LidarReceivedDataFormatter();
        else innerFormatter = new UltrasoundReceivedDataFormatter();
    }

    public (string extraMessage, double distanceMeasured) Format(JsonDocument data)
    {
        return innerFormatter.Format(data);
    }
}

public class UltrasoundReceivedDataFormatter : IInnerReceivedDataFormatter
{
    public (string extraMessage, double distanceMeasured) Format(JsonDocument data)
    {
        var dataRoot = data.RootElement;
        var extraMessage = dataRoot.GetProperty("extraMessage").GetString() ?? "";
        var distanceMeasured = dataRoot.GetProperty("distanceMeasured").GetDouble();

        return (extraMessage, distanceMeasured);
    }
}

public class LidarReceivedDataFormatter : IInnerReceivedDataFormatter
{
    public (string extraMessage, double distanceMeasured) Format(JsonDocument data)
    {
        string message = "";

        return (message, 0); // todo: calculate
    }
}
