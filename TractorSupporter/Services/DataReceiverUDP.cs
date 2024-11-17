using System.Net.Sockets;
using System.Net;
using TractorSupporter.Services.Interfaces;

namespace TractorSupporter.Services;

public partial class DataReceiverUDP : IDataReceiverAsync
{
    private readonly UdpClient _udpClient;
    private IPEndPoint _remoteIpEndpoint;

    private DataReceiverUDP(int port)
    {
        _udpClient = new UdpClient(port);
        _remoteIpEndpoint = new IPEndPoint(IPAddress.Any, port);
    }

    public async Task<byte[]> ReceiveDataAsync(CancellationToken token = default)
    {
        var result = await _udpClient.ReceiveAsync(token);

        _remoteIpEndpoint = result.RemoteEndPoint;
        return result.Buffer;
    }

    public string GetRemoteIpAddress()
    {
        return _remoteIpEndpoint.Address.ToString();
    }
}

#region Class structure
public partial class DataReceiverUDP
{
    private static Lazy<DataReceiverUDP>? _lazyInstance = null;

    public static DataReceiverUDP Instance
    {
        get 
        {
            if (_lazyInstance is null) throw new InvalidOperationException("Class should be initialized first!");

            return _lazyInstance.Value;
        }
    }

    public static DataReceiverUDP Initialize(int port)
    {
        if (_lazyInstance is null)
        {
            _lazyInstance = new(() => new DataReceiverUDP(port));
        }

        return _lazyInstance.Value;
    }
}
#endregion
