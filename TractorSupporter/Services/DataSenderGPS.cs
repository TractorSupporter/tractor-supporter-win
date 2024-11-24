using System.IO.Pipes;
using System.IO;
using System.Text.Json;

namespace TractorSupporter.Services;

public partial class DataSenderGPS
{
    private readonly GPSConnectionService _gpsConnectionService;

    private DataSenderGPS()
    {
        _gpsConnectionService = GPSConnectionService.Instance;
    }

    public async Task SendData(object jsonData)
    {
        if (_gpsConnectionService.IsConnecting)
            return;

        string jsonString = JsonSerializer.Serialize(jsonData);
        bool messageSent = false;

        while (!messageSent && _gpsConnectionService.ConnectingToGPSAllowed)
        {
            if (_gpsConnectionService.IsConnectedToGPS)
            {
                try
                {
                    _gpsConnectionService.WriteToPipe(jsonString);
                    messageSent = true;
                }
                catch (IOException ex)
                {
                    if (_gpsConnectionService.ConnectingToGPSAllowed)
                        await _gpsConnectionService.Connect();
                }
            }
            else
            {
                if (_gpsConnectionService.ConnectingToGPSAllowed)
                    await _gpsConnectionService.Connect();
            }
        }
    }
}

#region Class structure
public partial class DataSenderGPS
{
    private static readonly Lazy<DataSenderGPS> _lazyInstance = new(() => new DataSenderGPS());
    public static DataSenderGPS Instance => _lazyInstance.Value;
}
#endregion
