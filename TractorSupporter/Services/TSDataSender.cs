using System.IO.Pipes;
using System.IO;
using System.Text.Json;

namespace TractorSupporter.Services;

public class TSDataSender
{
    private static readonly Lazy<TSDataSender> _lazyInstance = new(() => new TSDataSender());
    public static TSDataSender Instance => _lazyInstance.Value;

    private readonly string _pipeName = "ts_pipe_to_gps";
    private NamedPipeClientStream _pipeClient;
    private StreamWriter _writer;
    private const int MaxRetryAttempts = 8;
    private const int RetryDelayMilliseconds = 1000;

    private TSDataSender()
    {
        ConnectToPipe(); // move to a connect button
    }

    private void ConnectToPipe()
    {
        _pipeClient = new NamedPipeClientStream(".", _pipeName, PipeDirection.Out);
        _pipeClient.Connect();
        _writer = new StreamWriter(_pipeClient) { AutoFlush = true };
    }

    public void SendData(object jsonData)
    {
        string jsonString = JsonSerializer.Serialize(jsonData);

        if (_pipeClient.IsConnected)
        {
            try
            {
                _writer.WriteLine(jsonString);
            }
            catch (IOException ex)
            {
                Console.WriteLine("Pipe is broken: " + ex.Message);
                RetrySendData(jsonString);
            }
        }
        else
        {
            Console.WriteLine("Pipe is not connected.");
            RetrySendData(jsonString);
        }
    }

    private void RetrySendData(string jsonString)
    {
        for (int attempt = 1; attempt <= MaxRetryAttempts; attempt++)
        {
            try
            {
                Console.WriteLine($"Attempting to reconnect... (Attempt {attempt})");
                ConnectToPipe();
                _writer.WriteLine(jsonString);
                Console.WriteLine("Reconnected and sent data successfully.");
                return;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Reconnection attempt {attempt} failed: {ex.Message}");
                Thread.Sleep(RetryDelayMilliseconds);
            }
        }

        Console.WriteLine("Failed to reconnect after multiple attempts.");
    }

    public void Dispose()
    {
        _writer?.Dispose();
        _pipeClient?.Dispose();
    }
}
