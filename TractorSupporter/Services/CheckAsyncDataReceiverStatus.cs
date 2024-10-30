using System.Configuration;
using System.Net.Sockets;

namespace TractorSupporter.Services;

public partial class CheckAsyncDataReceiverStatus<T>
{
    public event EventHandler<UpdateUdpConnectionStatusEventArgs> UpdateUdpConnectionStatus;
    private readonly int _receiveDataTimeoutMs;
    private bool _succeeded;
    private T? _data;

    public CheckAsyncDataReceiverStatus<T> CheckStatus(Task<T> dataReceiverTask)
    {
        try
        {
            Task finishedTask = Task.WhenAny(Task.Delay(_receiveDataTimeoutMs), dataReceiverTask).Result;

            if (finishedTask == dataReceiverTask)
            {
                _succeeded = true;
                _data = dataReceiverTask.Result;
            }
            else
            {
                _succeeded = false;
                _data = default(T);
            }

            UpdateUdpConnectionStatus.Invoke(this, new UpdateUdpConnectionStatusEventArgs(_succeeded));
            return this;
        }
        catch (AggregateException e) 
        {
            Exception inner = e.InnerException!;
            if (inner is TaskCanceledException)
            {

                _succeeded = false;
                _data = default(T);
            }
            else
                throw inner;
        }

        return this;
    }

    public bool TryGetResult(out T? data)
    {
        if (_succeeded)
        {
            _succeeded = false;
            data = _data;
            return true;
        }

        _data = default(T);
        data = _data;
        return false;
    }
}

#region Class structure
public partial class CheckAsyncDataReceiverStatus<T> where T : notnull
{
    private static readonly Lazy<CheckAsyncDataReceiverStatus<T>> _lazyInstance = new(() => new CheckAsyncDataReceiverStatus<T>());
    public static CheckAsyncDataReceiverStatus<T> Instance => _lazyInstance.Value;

    private CheckAsyncDataReceiverStatus() 
    {
        _data = default(T);
        _succeeded = false;
        _receiveDataTimeoutMs = int.Parse(ConfigurationManager.AppSettings["MaxLifeCheckESPResposeTime"]!);
    }
}
#endregion

public class UpdateUdpConnectionStatusEventArgs : EventArgs
{
    public bool ConnectionStatus { get; }

    public UpdateUdpConnectionStatusEventArgs(bool connectionStatus)
    {
        ConnectionStatus = connectionStatus;
    }
}