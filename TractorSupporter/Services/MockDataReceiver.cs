using System.Text;
using System.Text.Json;
using TractorSupporter.Services.Interfaces;

namespace TractorSupporter.Services;

public partial class MockDataReceiver
{
    public static string ExtraMessage { get; set; } = "extra message";
    public static double DistanceMeasured { get; set; } = 1000;


    public async Task<byte[]> ReceiveDataAsync(CancellationToken token = default)
    {
        await Task.Delay(300, token);

        var mockData = new 
        {
            extraMessage = ExtraMessage,
            distanceMeasured = DistanceMeasured
        };

        string jsonString = JsonSerializer.Serialize(mockData);
        byte[] data = Encoding.ASCII.GetBytes(jsonString);

        return await Task.FromResult(data);
    }

    public string GetRemoteIpAddress()
    {
        return "0.0.0.0";
    }
}

#region Class structure
public partial class MockDataReceiver : IDataReceiverAsync
{
    private static readonly Lazy<MockDataReceiver> _lazyInstance = new(() => new MockDataReceiver());
    public static MockDataReceiver Instance => _lazyInstance.Value;
    private MockDataReceiver() {}
}
#endregion
