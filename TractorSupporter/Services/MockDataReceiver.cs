using System.Text;
using System.Text.Json;
using TractorSupporter.Services.Interfaces;
using TractorSupporter.ViewModel;

namespace TractorSupporter.Services;

public interface IMockDataReceiver : IDataReceiverAsync
{
    public static string ExtraMessage { get; set; }
    public static double DistanceMeasured { get; set; }
    public void ChangeInnerMock(bool isLidar);
}

public partial class MockDataReceiver : IDataReceiverAsync, IMockDataReceiver
{
    private IMockDataConfigWindowViewModel _mockDataConfigWindowViewModel;

    public MockDataReceiver(IMockDataConfigWindowViewModel mockDataConfigWindowViewModel)
    {
        _mockDataConfigWindowViewModel = mockDataConfigWindowViewModel; 
    }

    public static string ExtraMessage { get; set; } = "extra message";
    public static double DistanceMeasured { get; set; } = 1000;
    private IInnerMockDataReceiver innerMockDataReceiver = new InnerMockLidarReceiver();

    public void ChangeInnerMock(bool isLidar)
    {
        if (isLidar)
        {
            _mockDataConfigWindowViewModel.DistanceMeasured = 10000;
            DistanceMeasured = 10000;
            innerMockDataReceiver = new InnerMockLidarReceiver();
        }
        else
        {
            _mockDataConfigWindowViewModel.DistanceMeasured = 1000;
            DistanceMeasured = 1000;
            innerMockDataReceiver = new InnerMockUltraSoundReceiver();
        }
    }

    public async Task<byte[]> ReceiveDataAsync(CancellationToken token = default)
    {
        return await innerMockDataReceiver.ReceiveDataAsync(DistanceMeasured, token);
    }

    public string GetRemoteIpAddress()
    {
        return "0.0.0.0";
    }
}

public interface IInnerMockDataReceiver
{
    public Task<byte[]> ReceiveDataAsync(double distance, CancellationToken token = default);
}

public class InnerMockUltraSoundReceiver : IInnerMockDataReceiver
{
    public static string ExtraMessage { get; set; } = "extra message";

    public async Task<byte[]> ReceiveDataAsync(double distance, CancellationToken token = default)
    {
        await Task.Delay(300, token);

        var mockData = new
        {
            extraMessage = ExtraMessage,
            distanceMeasured = distance
        };

        string jsonString = JsonSerializer.Serialize(mockData);
        byte[] data = Encoding.ASCII.GetBytes(jsonString);

        return await Task.FromResult(data);
    }
}

public class InnerMockLidarReceiver : IInnerMockDataReceiver
{
    public async Task<byte[]> ReceiveDataAsync(double distance, CancellationToken token = default)
    {
        StringBuilder measurementsSB = new StringBuilder();

        var degree_change = 1;

        for (int i = 0; i < 60; i++)
        {
            var angle = -30 + i * degree_change;

            measurementsSB.Append(angle.ToString());
            measurementsSB.Append(";");
            measurementsSB.Append(distance.ToString()); // mm
            measurementsSB.Append(";");
        }

        if (measurementsSB.Length > 0)
        {
            measurementsSB.Remove(measurementsSB.Length - 1, 1);
        }

        await Task.Delay(152, token);

        var mockData = new
        {
            sensor = "lidar",
            measurements = measurementsSB.ToString()
        };

        string jsonString = JsonSerializer.Serialize(mockData);
        byte[] data = Encoding.ASCII.GetBytes(jsonString);

        return await Task.FromResult(data);
    }
}
