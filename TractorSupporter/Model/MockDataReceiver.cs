using System.Text;
using System.Text.Json;

namespace TractorSupporter.Model;

public class MockDataReceiver : IDataReceiver
{
    public static string ExtraMessage { get; set; } = "extra message";
    public static double DistanceMeasured { get; set; } = 1000;
    private static double AlarmSignalThreshold = 970;
    private static double AvoidSignalThreshhold = 950;

    public byte[] ReceiveData()
    {
        var mockData = new
        {
            
            extraMessage = ExtraMessage,
            distanceMeasured = DistanceMeasured,
            avoidSignal = DistanceMeasured < AvoidSignalThreshhold,
            alarmSignal = DistanceMeasured < AlarmSignalThreshold
        };

        string jsonString = JsonSerializer.Serialize(mockData);
        return Encoding.ASCII.GetBytes(jsonString);
    }

    public string GetRemoteIpAddress()
    {
        return "0.0.0.0";
    }
}
