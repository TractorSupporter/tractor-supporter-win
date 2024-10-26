using System.Configuration;
using System.Net.Sockets;

namespace TractorSupporter.Services;

public partial class CheckAsyncDataReceiverStatus<T>
{
    private readonly int _receiveDataTimeoutMs;
    private bool _succeeded;
    private T? _data;

    public CheckAsyncDataReceiverStatus<T> CheckStatus(Task<T> dataReceiverTask)
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
        //_udpClient = new UdpClient();
    }

    //private T DefaultData => default(T);
}
#endregion
