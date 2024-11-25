using System.IO.Pipes;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace TractorSupporter.Services;

public partial class DataSenderGPS
{
    private readonly IGPSConnectionService _gpsConnectionService;

    private DataSenderGPS(IGPSConnectionService gpsConnection)
    {
        _gpsConnectionService = gpsConnection;
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
    private static readonly Lazy<DataSenderGPS> _lazyInstance = new(() => new DataSenderGPS(App.ServiceProvider.GetRequiredService<IGPSConnectionService>()));
    public static DataSenderGPS Instance => _lazyInstance.Value;
}
#endregion
