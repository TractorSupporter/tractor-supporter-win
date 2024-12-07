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
        double leastDistance = double.MaxValue;

        if (data.RootElement.TryGetProperty("measurements", out JsonElement measurements))
        {
            string[] measurementArray = measurements.GetString().Split(';');

            for (int i = 0; i < measurementArray.Length; i += 2)
            {
                (bool isValid, int angle, double distance) = ValidateMeasurement(measurementArray, i);

                if (!isValid)
                {
                    continue;
                }

                double distanceInFront = distance * Math.Cos(angle * Math.PI / 180);

                if (distanceInFront < leastDistance)
                {
                    leastDistance = distanceInFront;
                }
            }
        }

        double leastDistanceInCm = (double)(leastDistance / 10);
        return (message, leastDistanceInCm);
    }

    private (bool, int, double) ValidateMeasurement(string[] measurementArray, int i)
    {
        if (i + 1 >= measurementArray.Length)
        {
            return (false, 0, 0);
        }
        if (!int.TryParse(measurementArray[i], out int angle))
        {
            return (false, 0, 0);
        }
        if (!double.TryParse(measurementArray[i + 1], out double distance))
        {
            return (false, 0, 0);
        }

        return (true, angle, distance);
    }
}
