using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace TractorSupporter.Services;
    
public interface IDataReceiverGPS
{
    public Task StartReceivingAsync(CancellationToken token);
    public event EventHandler<bool> ReceivedAllowMakingDecision;
}

public class DataReceiverGPS : IDataReceiverGPS
{
    public event EventHandler<bool> ReceivedAllowMakingDecision;
    private readonly IGPSConnectionService _gpsConnectionService;

    public DataReceiverGPS(IGPSConnectionService gpsConnectionService) 
    {
        _gpsConnectionService = gpsConnectionService;
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



