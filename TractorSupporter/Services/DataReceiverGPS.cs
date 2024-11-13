using System.Text.Json;

namespace TractorSupporter.Services;
    
public partial class DataReceiverGPS 
{
    public event EventHandler<bool> ReceivedAllowMakingDecision;
    private readonly GPSConnectionService _gpsConnectionService;

    private DataReceiverGPS() 
    {
        _gpsConnectionService = GPSConnectionService.Instance;
    }

    public async Task StartReceivingAsync(CancellationToken token)
    {
        // check if ServerThreadService is set to true, stop receiving if false
        while (_gpsConnectionService.ConnectingToGPSAllowed)
        {
            while (_gpsConnectionService.IsConnectedToGPS)
            {
                try
                {
                    using (JsonDocument data = JsonDocument.Parse(await _gpsConnectionService.ReadFromPipe(token)))
                    {
                        JsonElement dataRoot = data.RootElement;
                        // change this to event! and avoiding service shall subcribe to it (name it: ReceivedAllowMakingDecision)
                        if (dataRoot.TryGetProperty("allowAvoidingDecision", out JsonElement blockAvoidingDecisionElement))
                            ReceivedAllowMakingDecision.Invoke(this, blockAvoidingDecisionElement.GetBoolean());
                        else
                            Console.WriteLine("Failed to recogize the input elements.");
                    }
                }
                catch (Exception e) 
                {
                    if (_gpsConnectionService.ConnectingToGPSAllowed)
                        await _gpsConnectionService.Connect();
                    else
                        return;
                }
            }

            if (_gpsConnectionService.ConnectingToGPSAllowed)
                await _gpsConnectionService.Connect();
        }
    }
}

#region Class structure
public partial class DataReceiverGPS
{
    private static readonly Lazy<DataReceiverGPS> _lazyInstance = new(() => new DataReceiverGPS());
    public static DataReceiverGPS Instance => _lazyInstance.Value;
}
#endregion


