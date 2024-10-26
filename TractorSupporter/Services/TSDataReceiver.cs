using System.IO.Pipes;
using System.IO;
using System.Text.Json;

namespace TractorSupporter.Services;
    
public partial class TSDataReceiver 
{
    private readonly AvoidingService _avoidingService;
    private readonly string _pipeName;
    private readonly NamedPipeServerStream _pipeServer;
    private readonly StreamReader _reader;

    private TSDataReceiver()
    {
        _avoidingService = AvoidingService.Instance;
        _pipeName = "ts_pipe_from_gps";
        _pipeServer = new NamedPipeServerStream(_pipeName, PipeDirection.In);
        _reader = new StreamReader(_pipeServer);
    }

    public async Task StartReceivingAsync()
    {
        _pipeServer.WaitForConnection();
        while (_pipeServer.IsConnected)
        {
            using (JsonDocument data = JsonDocument.Parse(await _reader.ReadLineAsync() ?? ""))
            {
                JsonElement dataRoot = data.RootElement;
                if (dataRoot.TryGetProperty("allowAvoidingDecision", out JsonElement blockAvoidingDecisionElement))
                    _avoidingService.AllowMakingDecision(blockAvoidingDecisionElement.GetBoolean());
                else
                    Console.WriteLine("Failed to recogize the input elements.");
            }
        }
    }
}

#region Class structure
public partial class TSDataReceiver : IDisposable
{
    private static readonly Lazy<TSDataReceiver> _lazyInstance = new(() => new TSDataReceiver());
    public static TSDataReceiver Instance => _lazyInstance.Value;

    public void Dispose()
    {
        _reader?.Dispose();
        _pipeServer?.Dispose();
    }
}
#endregion


