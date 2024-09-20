using System.IO.Pipes;
using System.IO;

namespace TractorSupporter.Model
{
    public class AvoidingCommandDataSender
    {
        private readonly string _pipeName;
        private NamedPipeClientStream _pipeClient;
        private StreamWriter _writer;
        private const int MaxRetryAttempts = 8;
        private const int RetryDelayMilliseconds = 1000;

        public AvoidingCommandDataSender(string pipeName)
        {
            _pipeName = pipeName;
            ConnectToPipe();
        }

        private void ConnectToPipe()
        {
            _pipeClient = new NamedPipeClientStream(".", _pipeName, PipeDirection.Out);
            _pipeClient.Connect();
            _writer = new StreamWriter(_pipeClient) { AutoFlush = true };
        }

        public void SendAvoidingCommand()
        {
            if (_pipeClient.IsConnected)
            {
                try
                {
                    string message = "avoid";
                    _writer.WriteLine(message);
                }
                catch (IOException ex)
                {
                    Console.WriteLine("Pipe is broken: " + ex.Message);
                    RetrySendCommand();
                }
            }
            else
            {
                Console.WriteLine("Pipe is not connected.");
                RetrySendCommand();
            }
        }

        private void RetrySendCommand()
        {
            for (int attempt = 1; attempt <= MaxRetryAttempts; attempt++)
            {
                try
                {
                    Console.WriteLine($"Attempting to reconnect... (Attempt {attempt})");
                    ConnectToPipe();
                    string message = "avoid";
                    _writer.WriteLine(message);
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
}
