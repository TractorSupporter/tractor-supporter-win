using System.Globalization;
using System.IO.Pipes;
using System.IO;

namespace TractorSupporter.Model;

public class UnlockCommandsDataReceiver : IDisposable
{
    private readonly NamedPipeServerStream _pipeServer;
    private readonly StreamReader _reader;
    public event Action<string> AvoidCommandUnblockReceived;
    CultureInfo culture = new CultureInfo("fr-FR");

    public UnlockCommandsDataReceiver(string pipeName)
    {
        _pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.In);
        _reader = new StreamReader(_pipeServer);
    }

    public async Task StartReceivingAsync()
    {
        _pipeServer.WaitForConnection();
        while (_pipeServer.IsConnected)
        {
            string message = await _reader.ReadLineAsync() ?? "";

            if (message == "unblock_avoid" || message == "start_avoid")
            {
                AvoidCommandUnblockReceived?.Invoke(message);
            }
            else
            {
                Console.WriteLine("Failed to recogize the input string.");
            }
        }
    }

    public void Dispose()
    {
        _reader?.Dispose();
        _pipeServer?.Dispose();
    }
}
