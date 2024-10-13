using System.IO.Pipes;
using System.IO;

namespace TractorSupporter.Services;
    
public partial class TSDataReceiver 
{
    
    private readonly AvoidingService _avoidingService;

    public async Task StartReceivingAsync()
    {
        _pipeServer.WaitForConnection();
        while (_pipeServer.IsConnected)
        {
            string message = await _reader.ReadLineAsync() ?? "";

            if (message == "unblock_avoid") 
                _avoidingService.UnblockMakingDecision();
            else                            
                Console.WriteLine("Failed to recogize the input string.");
        }
    }
}

#region Class structure
public partial class TSDataReceiver : IDisposable
{
    private static readonly Lazy<TSDataReceiver> _lazyInstance = new(() => new TSDataReceiver());
    public static TSDataReceiver Instance => _lazyInstance.Value;

    private readonly string _pipeName;
    private readonly NamedPipeServerStream _pipeServer;
    private readonly StreamReader _reader;

    public TSDataReceiver()
    {
        _avoidingService = AvoidingService.Instance;
        _pipeName = "ts_pipe_from_gps";
        _pipeServer = new NamedPipeServerStream(_pipeName, PipeDirection.In);
        _reader = new StreamReader(_pipeServer);
    }

    public void Dispose()
    {
        _reader?.Dispose();
        _pipeServer?.Dispose();
    }
}
#endregion


