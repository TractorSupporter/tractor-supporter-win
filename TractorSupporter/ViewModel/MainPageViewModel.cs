using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Net.Sockets;
using System.Text;
using System.Windows.Documents;
using System.Windows.Input;
using TractorSupporter.Model;
using TractorSupporter.Services;
using TractorSupporter.Services.Interfaces;

namespace TractorSupporter.ViewModel;

public class MainPageViewModel : BaseViewModel
{
    private bool _useMockData;
    private static MockDataConfigWindow _mockDataConfigWindow;
    private string _distanceToObstacle;
    private string _sendMessage;
    private string _ipSender;
    private string _ipDestination;
    private bool _isConnected;
    private bool _isUdpConnected;
    private bool _isGPSConnected;
    private ICommand _startConnectionCommand;
    private FlowDocument _receivedMessages;
    
    private INavigationService _navigationService;
    private IAlarmService _alarmService;
    private IAvoidingService _avoidingService;
    private readonly IGPSConnectionService _gpsConnectionService;
    private SettingsVisibilityService _settingsVisibilityService;
    private AppConfig _appConfig;
    private int _port;
    private string _ipAddress;
    private bool _isAvoidingMechanismTurnedOn;
    private bool _isAlarmMechanismTurnedOn;

    public MainPageViewModel(IAvoidingService avoiding, IAlarmService alarm, IGPSConnectionService gpsConnection)
    {
        _gpsConnectionService = gpsConnection;
        _avoidingService = avoiding;
        _navigationService = NavigationService.Instance;
        _alarmService = alarm;
        _settingsVisibilityService = SettingsVisibilityService.Instance;
        _receivedMessages = new FlowDocument();
        StartConnectionCommand = new RelayCommand(StartConnection);
        InitConfig();
        InitMockConfigWindow();
        _gpsConnectionService.ConnectedToGPSUpdated += OnUpdateConnectionToGPS; 
        ServerThreadService.Instance.UdpDataReceived += OnUdpDataReceived;
        CheckAsyncDataReceiverStatus<byte[]>.Instance.UpdateUdpConnectionStatus += OnUpdateUdpConnectionStatus;
        
        StandbyThreadService.Instance.StartStandby(_port, _useMockData);
    }

    public string DistanceToObstacle
    {
        get => _distanceToObstacle;
        set { _distanceToObstacle = value; OnPropertyChanged(nameof(DistanceToObstacle)); }
    }

    public FlowDocument ReceivedMessages
    {
        get => _receivedMessages;
        set { _receivedMessages = value; OnPropertyChanged(nameof(ReceivedMessages)); }
    }

    public string SendMessage
    {
        get => _sendMessage;
        set { _sendMessage = value; OnPropertyChanged(nameof(SendMessage)); }
    }

    public string IPSender
    {
        get => _ipSender;
        set { _ipSender = value; OnPropertyChanged(nameof(IPSender)); }
    }

    public bool IsUdpConnected
    {
        get => _isUdpConnected;
        set { _isUdpConnected = value; OnPropertyChanged(nameof(IsUdpConnected)); }
    }

    public bool IsConnected
    {
        get => _isConnected;
        set { _isConnected = value; OnPropertyChanged(nameof(IsConnected)); }
    }

    public bool IsGPSConnected
    {
        get => _isGPSConnected;
        set { _isGPSConnected = value; OnPropertyChanged(nameof(IsGPSConnected)); }
    }

    private string _invalidConnectionErrorMessage;
    public string InvalidConnectionErrorMessage
    {
        get => _invalidConnectionErrorMessage;
        set { _invalidConnectionErrorMessage = value; 
            OnPropertyChanged(nameof(InvalidConnectionErrorMessage)); }
    }

    public ICommand StartConnectionCommand
    {
        get => _startConnectionCommand;
        set
        {
            _startConnectionCommand = value;
            OnPropertyChanged(nameof(StartConnectionCommand));
        }
    }



    private void InitConfig()
    {
        _appConfig = ConfigAppJson.Instance.GetConfig();
        _port = _appConfig.Port;
        _alarmService.AlarmDistance = _appConfig.AlarmDistance;
        _avoidingService.AvoidingDistance = _appConfig.AvoidingDistance;
        _isAvoidingMechanismTurnedOn = _appConfig.IsAvoidingMechanismTurnedOn;
        _isAlarmMechanismTurnedOn = _appConfig.IsAlarmMechanismTurnedOn;
        ServerThreadService.Instance.IsAvoidingMechanismTurnedOn = _isAvoidingMechanismTurnedOn;
        ServerThreadService.Instance.IsAlarmMechanismTurnedOn = _isAlarmMechanismTurnedOn;
    }

    private void InitMockConfigWindow()
    {
        _useMockData = bool.Parse(ConfigurationManager.AppSettings["UseMockData"]!);
        if (_useMockData && (_mockDataConfigWindow == null || !_mockDataConfigWindow.IsVisible))
        {
            _mockDataConfigWindow = new MockDataConfigWindow(App.ServiceProvider.GetRequiredService<IMockDataConfigWindowViewModel>());
            _mockDataConfigWindow.Show();
        }
    }

    public ICommand SendMessageCommand => new RelayCommand(SendMessageExecute);

    private void AddParagraphToReceivedMessages(string extraMessage)
    { 
        string datetime = DateTime.Now.ToString("hh:mm:ss");
        extraMessage = "time: " + datetime + " | " + extraMessage;
        Paragraph newParagraph = new Paragraph(new Run(extraMessage));

        if (ReceivedMessages.Blocks.FirstBlock != null)
        {
            ReceivedMessages.Blocks.InsertBefore(ReceivedMessages.Blocks.FirstBlock, newParagraph);
        }
        else
        {
            ReceivedMessages.Blocks.Add(newParagraph);
        }
    }
    private void SendMessageExecute(object parameter)
    {
        UdpClient udpClient = new UdpClient();
        udpClient.Connect(_ipDestination, Convert.ToInt16(parameter));
        byte[] dataToSend = Encoding.ASCII.GetBytes(SendMessage);
        udpClient.Send(dataToSend, dataToSend.Length);
    }

    private bool ValidateStartConnection()
    {
        return IsUdpConnected;
    }

    private void StartConnection(object parameter)
    {
        if (!ValidateStartConnection() && !IsConnected)
        {
            InvalidConnectionErrorMessage = "Connect to ESP32 first!";
            return;
        }
        else
        {
            InvalidConnectionErrorMessage = string.Empty;
        }

        IsConnected = !IsConnected;
        _settingsVisibilityService.IsSettingsVisible = !IsConnected;
        HistoryVisibilityService.Instance.IsHistoryVisible = !IsConnected;

        _appConfig = ConfigAppJson.Instance.GetConfig();
        _port = _appConfig.Port;

        if (IsConnected)
        {
            StandbyThreadService.Instance.StopStandby();
            ServerThreadService.Instance.StartServer(_port, _useMockData);
        }
        else
        {
            ServerThreadService.Instance.StopServer();
            StandbyThreadService.Instance.StartStandby(_port, _useMockData);
        }
    }

    private void OnUdpDataReceived(object? sender, UdpDataReceivedEventArgs e)
    {
        IPSender = e.IpSender;
        DistanceToObstacle = e.DistanceMeasured.ToString();
        AddParagraphToReceivedMessages(e.ExtraMessage);
    }

    private void OnUpdateUdpConnectionStatus(object? sender, UpdateUdpConnectionStatusEventArgs e)
    {
        IsUdpConnected = e.ConnectionStatus;
    }

    private void OnUpdateConnectionToGPS(object? sender, bool isConnected)
    {
        IsGPSConnected = isConnected;
    }
}
