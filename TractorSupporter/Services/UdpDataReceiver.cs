using System.Net.Sockets;
using System.Net;
using TractorSupporter.Services.Interfaces;

namespace TractorSupporter.Services;

public partial class UdpDataReceiver : IDataReceiverAsync
{
    private readonly UdpClient _udpClient;
    private IPEndPoint _remoteIpEndpoint;

    private UdpDataReceiver(int port)
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
public partial class UdpDataReceiver
{
    private static Lazy<UdpDataReceiver>? _lazyInstance = null;

    public static UdpDataReceiver Instance
    {
        get 
        {
            if (_lazyInstance is null) throw new InvalidOperationException("Class should be initialized first!");

            return _lazyInstance.Value;
        }
    }

    public static UdpDataReceiver Initialize(int port)
    {
        if (_lazyInstance is null)
        {
            _lazyInstance = new(() => new UdpDataReceiver(port));
        }

        return _lazyInstance.Value;
    }
}
#endregion
