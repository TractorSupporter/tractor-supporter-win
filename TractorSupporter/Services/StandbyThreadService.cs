using TractorSupporter.Services.Interfaces;

namespace TractorSupporter.Services;

public partial class StandbyThreadService
{
    private bool _useMockData;
    private Thread _standbyThread;
    private bool _shouldRun;
    private IDataReceiverAsync _dataReceiverESP;
    private CheckAsyncDataReceiverStatus<byte[]> _checkDataReceiverStatus;
    private CancellationTokenSource _cancellationTokenSource;
    private int _currentPort;

    private StandbyThreadService()
    {
        _checkDataReceiverStatus = CheckAsyncDataReceiverStatus<byte[]>.Instance;
        _cancellationTokenSource = new CancellationTokenSource();

    }

    public void StopStandby()
    {
        _shouldRun = false;
        _cancellationTokenSource.Cancel();
    }

    public void StartStandby(int port, bool useMockData)
    {
        _useMockData = useMockData;
        _currentPort = port;
        _standbyThread = new Thread(() => StandbyThread(_cancellationTokenSource.Token));
        _shouldRun = true;
        _standbyThread.IsBackground = true;
        _dataReceiverESP = useMockData ? MockDataReceiver.Instance : DataReceiverUDP.Initialize(port);
        _standbyThread.Start();
    }

    private void StandbyThread(CancellationToken token)
    {
        while (_shouldRun && !token.IsCancellationRequested)
        {
            _checkDataReceiverStatus.CheckStatus(_dataReceiverESP.ReceiveDataAsync(token));

            if (!DataSenderUDP.IsInitialized && _checkDataReceiverStatus.TryGetResult(out byte[]? x))
                DataSenderUDP.Initialize(_dataReceiverESP.GetRemoteIpAddress(), _currentPort);
        }
    }
}

#region Class structure
public partial class StandbyThreadService
{
    private static readonly Lazy<StandbyThreadService> _lazyInstance = new(() => new StandbyThreadService());
    public static StandbyThreadService Instance => _lazyInstance.Value;
}
#endregion