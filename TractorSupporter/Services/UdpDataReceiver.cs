using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TractorSupporter.Services.Interfaces;

namespace TractorSupporter.Services
{
    public class UdpDataReceiver : IDataReceiver
    {
        private readonly UdpClient _udpClient;
        private IPEndPoint _remoteIpEndpoint;

        public UdpDataReceiver(int port)
        {
            _udpClient = new UdpClient(port);
            _remoteIpEndpoint = new IPEndPoint(IPAddress.Any, port);
        }

        public byte[] ReceiveData()
        {
            return _udpClient.Receive(ref _remoteIpEndpoint);
        }

        public string GetRemoteIpAddress()
        {
            return _remoteIpEndpoint.Address.ToString();
        }
    }
}
