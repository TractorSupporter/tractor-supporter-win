namespace TractorSupporter.Services.Interfaces;

public interface IDataReceiverAsync
{
    Task<byte[]> ReceiveDataAsync();
    string GetRemoteIpAddress();
}
