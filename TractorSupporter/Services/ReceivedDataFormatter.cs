using System.Text.Json;
using TractorSupporter.Model;

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
    private ConfigAppJson configGetter = ConfigAppJson.Instance;
    private AppConfig appConfig = null;

    private const double distanceThreshold = 1000;
    public (string extraMessage, double distanceMeasured) Format(JsonDocument data)
    {
        appConfig = configGetter.GetConfig();

        string message = "";
        double leastDistance = double.MaxValue;

        if (data.RootElement.TryGetProperty("measurements", out JsonElement measurements))
        {
            string[] measurementArray = measurements.GetString().Split(';');

            for (int i = 0; i < measurementArray.Length; i += 2)
            {
                (bool isValid, double angle, double distance) = ValidateMeasurement(measurementArray, i);

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

    private (bool, double, double) ValidateMeasurement(string[] measurementArray, int i)
    {
        if (i + 1 >= measurementArray.Length)
        {
            return (false, 0, 0);
        }
        if (!double.TryParse(measurementArray[i].Replace('.', ','), out double angle))
        {
            return (false, 0, 0);
        }
        if (!double.TryParse(measurementArray[i + 1].Replace('.', ','), out double distance))
        {
            return (false, 0, 0);
        }
        if(distance< distanceThreshold)
        {
            return (false, 0, 0);
        }


        var max_dist = appConfig.VehicleWidth * 1000 / 2 / Math.Sin(Math.Abs(angle) * Math.PI / 180);
        if (distance > max_dist)
        {
            return (false, 0, 0);
        }
        else
        {
            int xd = 3;
        }

        return (true, angle, distance);
    }

}
