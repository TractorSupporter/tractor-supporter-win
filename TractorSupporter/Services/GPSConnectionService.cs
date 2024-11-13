using System.IO.Pipes;
using System.IO;

namespace TractorSupporter.Services;

public partial class GPSConnectionService
{
    private readonly string _pipeName;
    private NamedPipeClientStream _pipeClient;
    private StreamReader _reader;
    private StreamWriter _writer;

    public event EventHandler<bool> ConnectedToGPSUpdated;
    //private readonly string _inputPipeName;
    //private NamedPipeServerStream _inputPipeServer;
    //private StreamReader _inputReader;
    //private readonly string _outputPipeName;
    //private NamedPipeClientStream _outputPipeClient;
    //private StreamWriter _outputWriter;
    private readonly SemaphoreSlim _connectToGPSSemaphore;
    private CancellationTokenSource _cancellationTokenSource;
    public bool IsConnecting { get; private set; }
    public bool ConnectingToGPSAllowed { get; private set; }
    //public bool IsConnectedToGPS => _outputPipeClient.IsConnected && _inputPipeServer.IsConnected;
    public bool IsConnectedToGPS => _pipeClient.IsConnected;

    public async Task Connect()
    {
        ConnectingToGPSAllowed = true;
        if (_cancellationTokenSource.IsCancellationRequested)
            _cancellationTokenSource = new CancellationTokenSource();
        await StayConnectedToGPSAsync(_cancellationTokenSource.Token);
    }

    public void Disconnect()
    {
        ConnectingToGPSAllowed = false;
        _cancellationTokenSource.Cancel();

        if (_pipeClient.IsConnected)
        {
            _pipeClient.Dispose();
        }

        //if (_outputPipeClient.IsConnected)
        //    _outputPipeClient.Close();
        //if (_inputPipeServer.IsConnected)
        //    _inputPipeServer.Close();

        App.Current.Dispatcher.Invoke(() =>
        {
            ConnectedToGPSUpdated.Invoke(this, false);
        });
    }

    private GPSConnectionService()
    {
        _pipeName = "ts_gps_pipe";
        _pipeClient = new NamedPipeClientStream(".", _pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
        _reader = new StreamReader(_pipeClient);

        //_inputPipeName = "ts_pipe_from_gps";
        //_inputPipeServer = new NamedPipeServerStream(_inputPipeName, PipeDirection.In);
        //_inputReader = new StreamReader(_inputPipeServer);

        //_outputPipeName = "ts_pipe_to_gps";
        //_outputPipeClient = new NamedPipeClientStream(".", _outputPipeName, PipeDirection.Out);

        _cancellationTokenSource = new CancellationTokenSource();
        _connectToGPSSemaphore = new SemaphoreSlim(1, 1);
        ConnectingToGPSAllowed = false;
    }

    private async Task StayConnectedToGPSAsync(CancellationToken token)
    {
        if (IsConnectedToGPS || !ConnectingToGPSAllowed)
            return;

        await _connectToGPSSemaphore.WaitAsync();
        try
        {
            if (IsConnectedToGPS || !ConnectingToGPSAllowed)
                return;

            IsConnecting = true;

            _pipeClient.Dispose();
            _pipeClient = new NamedPipeClientStream(".", _pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
            _reader = new StreamReader(_pipeClient);

            //if (_outputPipeClient.IsConnected)
            //    _outputPipeClient.Close();
            //if (_inputPipeServer.IsConnected)
            //    _inputPipeServer.Disconnect();


            App.Current.Dispatcher.Invoke(() =>
            {
                ConnectedToGPSUpdated.Invoke(this, IsConnectedToGPS);
            });

            await _pipeClient.ConnectAsync(token);
            _writer = new StreamWriter(_pipeClient) { AutoFlush = true };

            //if (!_outputPipeClient.IsConnected)
            //{
            //    await _outputPipeClient.ConnectAsync(token);
            //}
            //if (!_inputPipeServer.IsConnected)
            //{
            //    await _inputPipeServer.WaitForConnectionAsync(token);
            //}

            //_outputWriter = new StreamWriter(_outputPipeClient) { AutoFlush = true };

            App.Current.Dispatcher.Invoke(() =>
            {
                ConnectedToGPSUpdated.Invoke(this, IsConnectedToGPS);
            });
        }
        catch (Exception e) 
        {
            var aggregateEx = e as AggregateException;
            if (aggregateEx is not null)
            {
                Exception inner = e.InnerException!;
                if (inner is not TaskCanceledException)
                    throw inner;
            }

            var opCancEx = e as OperationCanceledException;
            if (opCancEx is null)
                throw e;
        }
        finally
        {
            IsConnecting = false;
            _connectToGPSSemaphore.Release();
        }
    }

    public void WriteToPipe(string data)
    {
        _writer.WriteLineAsync(data);
    }

    public async Task<string> ReadFromPipe(CancellationToken token)
    {
        return await _reader.ReadLineAsync(token) ?? "";
    }
}

#region Class structure
public partial class GPSConnectionService
{
    private static readonly Lazy<GPSConnectionService> _lazyInstance = new(() => new GPSConnectionService());
    public static GPSConnectionService Instance => _lazyInstance.Value;
}
#endregion
