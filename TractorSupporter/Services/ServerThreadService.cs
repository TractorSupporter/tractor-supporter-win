using System.Text.Json;
using System.Text;
using TractorSupporter.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using TractorSupporter.Model.Enums;

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
    private IDataReceiverGPS _dataReceiverGPS;
    private IDataSenderGPS _dataSenderGPS;
    private IDataSenderUDP _dataSenderUDP;
    private IReceivedDataFormatter _receivedDataFormatter;
    private ILidarDistanceService _lidarDistanceService;
    private CheckAsyncDataReceiverStatus<byte[]> _checkDataReceiverStatus;
    private CancellationTokenSource _cancellationTokenSource;
    private int _currentPort;
    private double speed;

    public bool IsAvoidingMechanismTurnedOn { get; set; }
    public bool IsAlarmMechanismTurnedOn { get; set; }

    public bool IsConnected => _isConnected;

    public ServerThreadService(IAvoidingService avoiding, IAlarmService alarm, IGPSConnectionService gpsConnection, IReceivedDataFormatter receivedDataFormatter, IMockDataReceiver mockDataReceiver, ILidarDistanceService lidarDistanceService) 
    {
        _mockDataReceiver = mockDataReceiver;
        _receivedDataFormatter = receivedDataFormatter;
        _gpsConnectionService = gpsConnection;
        _lidarDistanceService = lidarDistanceService;
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
        _dataReceiverGPS = App.ServiceProvider.GetRequiredService<IDataReceiverGPS>();
        _dataSenderGPS = App.ServiceProvider.GetRequiredService<IDataSenderGPS>(); ;
        _dataSenderUDP = DataSenderUDP.Instance;

        _ = _dataSenderUDP.SendDataAsync(new { shouldRun = true, config = ConfigAppJson.Instance.GetConfig().SelectedSensorType });
        _ = _gpsConnectionService.Connect();
        _ = _dataReceiverGPS.StartReceivingAsync(token);
        _dataReceiverGPS.ReceivedVehicleSpeed += ProcessVehicleSpeed;

        while (_isConnected && !token.IsCancellationRequested)
        {   
            if (!_checkDataReceiverStatus.CheckStatus(_dataReceiverESP.ReceiveDataAsync(token)).TryGetResult(out byte[]? result))
            {
                continue;
            }

            ProcessReceivedData(result!);
            Thread.Sleep(166);
        }
    }

    private void ProcessVehicleSpeed(object? sender, double speed)
    {
        // m/s to mm/s
        this.speed = speed * 1000;
    }

    private void FormatReceivedData(JsonDocument data, ref double angle, ref double distanceMeasured, ref string extraMessage, ref bool shouldAvoid, ref bool shouldAlarm)
    {
        if (_receivedDataFormatter.Format(data) is UltrasoundResult ultrasound)
        {
            FormatUltrasoundData(ultrasound, ref distanceMeasured, ref extraMessage, ref shouldAvoid, ref shouldAlarm);
        }
        else if (_receivedDataFormatter.Format(data) is LidarResult lidar)
        {
            FormatLidarData(lidar, ref angle, ref distanceMeasured, ref extraMessage, ref shouldAvoid, ref shouldAlarm);
        }
    }

    private void FormatUltrasoundData(UltrasoundResult ultrasound, ref double distanceMeasured, ref string extraMessage, ref bool shouldAvoid, ref bool shouldAlarm)
    {
        distanceMeasured = ultrasound.DistanceMeasured;
        extraMessage = ultrasound.ExtraMessage;
        shouldAvoid = IsAvoidingMechanismTurnedOn ? _avoidingService.MakeAvoidingDecision(distanceMeasured) : false;
        shouldAlarm = IsAlarmMechanismTurnedOn ? _alarmService.MakeAlarmDecision(distanceMeasured) : false;
    }

    private void FormatLidarData(LidarResult lidar, ref double angle, ref double distanceMeasured, ref string extraMessage, ref bool shouldAvoid, ref bool shouldAlarm)
    {
        extraMessage = lidar.ExtraMessage;
        distanceMeasured = _lidarDistanceService.FindClosestDistance(lidar.Measurements, speed, ref angle);
        shouldAvoid = IsAvoidingMechanismTurnedOn ? _avoidingService.MakeLidarAvoidingDecision(distanceMeasured) : false;
        shouldAlarm = IsAlarmMechanismTurnedOn ? _alarmService.MakeLidarAlarmDecision(distanceMeasured) : false;
    }

    private void ProcessReceivedData(byte[] receivedBytes)
    {
        string serializedData = Encoding.ASCII.GetString(receivedBytes);
        using (JsonDocument data = JsonDocument.Parse(serializedData))
        {
            bool shouldAvoid = false;
            bool shouldAlarm = false;
            double distanceMeasured = 0;
            string extraMessage = "";
            string ipSender = _dataReceiverESP.GetRemoteIpAddress();
            
            double angle = 0;
            FormatReceivedData(data, ref angle, ref distanceMeasured, ref extraMessage, ref shouldAvoid, ref shouldAlarm);
            TypeTurn turnDirection = GetTurnDirection(angle > 0);
            

            if (!_cancellationTokenSource.IsCancellationRequested)
                _ = _dataSenderGPS.SendData(new
                {
                    angle,
                    turnDirection,
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

    private TypeTurn GetTurnDirection(bool isTurnLeft)
    {
        if (ConfigAppJson.Instance.GetConfig().SelectedTurnDirection == TypeTurn.Auto)
            return isTurnLeft ? TypeTurn.Left : TypeTurn.Right;
        else
            return ConfigAppJson.Instance.GetConfig().SelectedTurnDirection;
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
        App.ServiceProvider.GetRequiredService<IMockDataReceiver>(),
        App.ServiceProvider.GetRequiredService<ILidarDistanceService>()
        ));
        
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
