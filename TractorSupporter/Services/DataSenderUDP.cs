using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace TractorSupporter.Services;

public partial class DataSenderUDP
{
    private readonly UdpClient _udpClient;
    private int _port;
    private string _destinationIP;
    private DataSenderUDP(string destinationIP, int port)
    {
        _udpClient = new UdpClient();
        _port = port;
        _destinationIP = destinationIP;
        _udpClient.Connect(_destinationIP, _port);
    }

    public async Task SendDataAsync(object jsonData) // TODO: consider sending 3 times (for making sure that UDP won't fail)
    {
        byte[] data = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(jsonData));

        foreach (var _ in Enumerable.Range(0, 3))
            await _udpClient.SendAsync(data);
    }
}

#region Class structure
public partial class DataSenderUDP
{
    public static bool IsInitialized => _lazyInstance != null;

    private static Lazy<DataSenderUDP>? _lazyInstance = null;

    public static DataSenderUDP Instance
    {
        get
        {
            if (_lazyInstance is null) throw new InvalidOperationException("Class should be initialized first!");

            return _lazyInstance.Value;
        }
    }

    public static DataSenderUDP Initialize(string destinationIP, int port)
    {
        if (_lazyInstance is null)
        {
            _lazyInstance = new(() => new DataSenderUDP(destinationIP, port));
        }

        return _lazyInstance.Value;
    }
}
#endregion
