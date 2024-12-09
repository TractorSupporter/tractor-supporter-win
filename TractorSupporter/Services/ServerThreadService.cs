using System.Text.Json;
using System.Text;
using TractorSupporter.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace TractorSupporter.Services;

public partial class ServerThreadService
{
    private bool _useMockData;
    public EventHandler<UdpDataReceivedEventArgs> UdpDataReceived;
    private Thread _serverThread;
    private bool _isConnected;
    private IGPSConnectionService _gpsConnectionService;
    private IDataReceiverAsync _dataReceiverESP;
    private IMockDataReceiver _mockDataReceiver;
    private IAvoidingService _avoidingService;
    private IAlarmService _alarmService;
    private DataReceiverGPS _dataReceiverGPS;
    private DataSenderGPS _dataSenderGPS;
    private IDataSenderUDP _dataSenderUDP;
    private IReceivedDataFormatter _receivedDataFormatter;
    private CheckAsyncDataReceiverStatus<byte[]> _checkDataReceiverStatus;
    private CancellationTokenSource _cancellationTokenSource;
    private int _currentPort;

    public bool IsAvoidingMechanismTurnedOn { get; set; }
    public bool IsAlarmMechanismTurnedOn { get; set; }

    public bool IsConnected => _isConnected;

    public ServerThreadService(IAvoidingService avoiding, IAlarmService alarm, IGPSConnectionService gpsConnection, IReceivedDataFormatter receivedDataFormatter, IMockDataReceiver mockDataReceiver) 
    {
        _mockDataReceiver = mockDataReceiver;
        _receivedDataFormatter = receivedDataFormatter;
        _gpsConnectionService = gpsConnection;
        _alarmService = alarm;
        _avoidingService = avoiding;
        _checkDataReceiverStatus = CheckAsyncDataReceiverStatus<byte[]>.Instance;
        _cancellationTokenSource = new CancellationTokenSource(); 
    }

    public void StopServer()
    {
        _ = _dataSenderUDP.SendDataAsync(new { shouldRun = false });
        _gpsConnectionService.Disconnect();
        _isConnected = false;
        _cancellationTokenSource.Cancel();
    }

    public void StartServer(int port, bool useMockData)
    {
        _useMockData = useMockData;
        _currentPort = port;
        _cancellationTokenSource = new CancellationTokenSource();
        _serverThread = new Thread(() => ServerThread(_cancellationTokenSource.Token));
        _dataReceiverESP = useMockData ? _mockDataReceiver : DataReceiverUDP.Initialize(port);
        _isConnected = true;
        _serverThread.IsBackground = true;
        _serverThread.Start();
    }

    private void ServerThread(CancellationToken token)
    {
        _dataReceiverGPS = DataReceiverGPS.Instance;
        _dataSenderGPS = DataSenderGPS.Instance;
        _dataSenderUDP = DataSenderUDP.Instance;

        _ = _dataSenderUDP.SendDataAsync(new { shouldRun = true, config = ConfigAppJson.Instance.GetConfig().SelectedSensorType });
        _ = _gpsConnectionService.Connect();
        _ = _dataReceiverGPS.StartReceivingAsync(token);

        while (_isConnected && !token.IsCancellationRequested)
        {   
            if (!_checkDataReceiverStatus.CheckStatus(_dataReceiverESP.ReceiveDataAsync(token)).TryGetResult(out byte[]? result))
            {
                continue;
            }

            ProcessReceivedData(result!);
        }
    }
    
    private void ProcessReceivedData(byte[] receivedBytes)
    {
        string serializedData = Encoding.ASCII.GetString(receivedBytes);
        using (JsonDocument data = JsonDocument.Parse(serializedData))
        {
            (string extraMessage, double distanceMeasured) = _receivedDataFormatter.Format(data);
            string ipSender = _dataReceiverESP.GetRemoteIpAddress();

            bool shouldAvoid = IsAvoidingMechanismTurnedOn ? _avoidingService.MakeAvoidingDecision(distanceMeasured) : false;
            bool shouldAlarm = IsAlarmMechanismTurnedOn ? _alarmService.MakeAlarmDecision(distanceMeasured) : false;

            if (!_cancellationTokenSource.IsCancellationRequested)
                _ = _dataSenderGPS.SendData(new
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
    private static readonly Lazy<ServerThreadService> _lazyInstance = new(() => new ServerThreadService(
        App.ServiceProvider.GetRequiredService<IAvoidingService>(),
        App.ServiceProvider.GetRequiredService<IAlarmService>(),
        App.ServiceProvider.GetRequiredService<IGPSConnectionService>(),
        App.ServiceProvider.GetRequiredService<IReceivedDataFormatter>(), 
        App.ServiceProvider.GetRequiredService<IMockDataReceiver>()));
        
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
