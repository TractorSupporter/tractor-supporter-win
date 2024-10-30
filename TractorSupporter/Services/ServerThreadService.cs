using System.Diagnostics;
using System.Text.Json;
using System.Text;
using TractorSupporter.Services.Interfaces;

namespace TractorSupporter.Services;

public partial class ServerThreadService
{
    public EventHandler<UdpDataReceivedEventArgs> UdpDataReceived;
    private Thread _serverThread;
    private bool _isConnected;
    private IDataReceiverAsync _dataReceiverESP;
    private AvoidingService _avoidingService;
    private AlarmService _alarmService;
    private TSDataReceiver _dataReceiverTS;
    private TSDataSender _dataSender;
    private CheckAsyncDataReceiverStatus<byte[]> _checkDataReceiverStatus;
    private CancellationTokenSource _cancellationTokenSource;

    private ServerThreadService() 
    {
        _checkDataReceiverStatus = CheckAsyncDataReceiverStatus<byte[]>.Instance;
        _cancellationTokenSource = new CancellationTokenSource(); 
    }

    public void StopServer()
    {
        _isConnected = false;
        _cancellationTokenSource?.Cancel();
    }

    public void StartServer(int port, bool useMockData)
    {
        _serverThread = new Thread(() => ServerThread(_cancellationTokenSource.Token));
        _dataReceiverESP = useMockData ? MockDataReceiver.Instance : UdpDataReceiver.Initialize(port);
        _isConnected = true;
        _serverThread.IsBackground = true;
        _serverThread.Start();
    }

    private void ServerThread(CancellationToken token)
    {
        _alarmService = AlarmService.Instance;
        _avoidingService = AvoidingService.Instance;
        _dataReceiverTS = TSDataReceiver.Instance;
        _dataSender = TSDataSender.Instance;
        

        _ = _dataReceiverTS.StartReceivingAsync();

        while (_isConnected && !token.IsCancellationRequested)
        {
            if (!_checkDataReceiverStatus.CheckStatus(_dataReceiverESP.ReceiveDataAsync(token)).TryGetResult(out byte[]? result))
            {
                Console.WriteLine("Failed to get data for 5 seconds.");

                continue;
            }

            ProcessReceivedData(result!);
        }

        // TODO add checking signal even if app is disconnected from AgOpenGPS
    }
    
    private void ProcessReceivedData(byte[] receivedBytes)
    {
        string serializedData = Encoding.ASCII.GetString(receivedBytes);
        using (JsonDocument data = JsonDocument.Parse(serializedData))
        {
            var dataRoot = data.RootElement;
            var extraMessage = dataRoot.GetProperty("extraMessage").GetString() ?? "";
            var distanceMeasured = dataRoot.GetProperty("distanceMeasured").GetDouble();

            string ipSender = _dataReceiverESP.GetRemoteIpAddress();

            bool shouldAvoid = _avoidingService.MakeAvoidingDecision(distanceMeasured);
            bool shouldAlarm = _alarmService.MakeAlarmDecision(distanceMeasured);

            _dataSender.SendData(new
            {
                shouldAvoid,
                shouldAlarm,
                distanceMeasured
            });

            App.Current.Dispatcher.Invoke(() =>
            {
                // Trigger event to update the view model.
                UdpDataReceived?.Invoke(this, new UdpDataReceivedEventArgs(ipSender, extraMessage, distanceMeasured));
            });
        }
    }
}

#region Class structure
public partial class ServerThreadService
{
    private static readonly Lazy<ServerThreadService> _lazyInstance = new(() => new ServerThreadService());
    public static ServerThreadService Instance => _lazyInstance.Value;
    
}
#endregion

public class UdpDataReceivedEventArgs : EventArgs
{
    public string IpSender { get; }
    public string ExtraMessage { get; }
    public double DistanceMeasured { get; }

    public UdpDataReceivedEventArgs(string ipSender, string extraMessage, double distanceMeasured)
    {
        IpSender = ipSender;
        ExtraMessage = extraMessage;
        DistanceMeasured = distanceMeasured;
    }
}
