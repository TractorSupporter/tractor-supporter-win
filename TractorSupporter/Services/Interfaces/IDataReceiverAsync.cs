namespace TractorSupporter.Services.Interfaces;

public interface IDataReceiverAsync
{
    Task<byte[]> ReceiveDataAsync(CancellationToken token = default);
    string GetRemoteIpAddress();
}
