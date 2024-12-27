using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using TractorSupporter.Model;

namespace TractorSupporter.Services;
    
public interface IDataReceiverGPS
{
    public Task StartReceivingAsync(CancellationToken token);
    public event EventHandler<bool> ReceivedAvoidingDecisionState;
    public event EventHandler<double> ReceivedVehicleSpeed;
}

public class DataReceiverGPS : IDataReceiverGPS
{
    public event EventHandler<bool> ReceivedAvoidingDecisionState;
    public event EventHandler<double> ReceivedVehicleWidth;
    public event EventHandler<double> ReceivedVehicleSpeed;
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
                        {
                            ReceivedAvoidingDecisionState.Invoke(this, blockAvoidingDecisionElement.GetBoolean());
                        }
                        if (dataRoot.TryGetProperty("vehicleWidth", out JsonElement vehicleWidthElement))
                        {
                            AppConfig appConfig = ConfigAppJson.Instance.GetConfig();
                            ConfigAppJson.Instance.CreateJson(appConfig.Port.ToString(), appConfig.IpAddress, appConfig.IsAvoidingMechanismTurnedOn,
                                appConfig.IsAlarmMechanismTurnedOn, appConfig.SelectedSensorType, appConfig.AvoidingDistance, appConfig.AlarmDistance,
                                appConfig.Language, appConfig.SelectedTurnDirection, vehicleWidthElement.GetDouble());
                        }
                        if (dataRoot.TryGetProperty("speed", out JsonElement speedElement))
                        {
                            ReceivedVehicleSpeed.Invoke(this, speedElement.GetDouble());
                        }
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



