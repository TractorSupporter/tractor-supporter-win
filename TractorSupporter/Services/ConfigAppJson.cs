using System.IO;
using System.Text.Json;
using TractorSupporter.Model;
using TractorSupporter.Model.Enums;
using TractorSupporter.Services.Interfaces;

namespace TractorSupporter.Services;

public partial class ConfigAppJson
{
    private AppConfig _appConfig;
    readonly string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
    readonly string fileName = "config.json";

    public void CreateJson(string port, string ipAddress, bool option1, bool option2, TypeSensor selectedTypeSensor)
    {
        var config = new AppConfig
        {
            Port = int.Parse(port),
            IpAddress = ipAddress,
            Option1 = option1,
            Option2 = option2,
            SelectedSensorType = selectedTypeSensor
        };

        string jsonString = JsonSerializer.Serialize(config);
        string filePath = Path.Combine(baseDirectory, fileName);

        File.WriteAllText(filePath, jsonString);
    }

    public AppConfig ReadJson()
    {
        string filePath = Path.Combine(baseDirectory, fileName);

        if (!File.Exists(filePath))
        {
            return null;
        }

        string jsonString = File.ReadAllText(filePath);
        _appConfig = JsonSerializer.Deserialize<AppConfig>(jsonString);

        return _appConfig;
    }

    public AppConfig GetConfig()
    {
        if (_appConfig == null)
        {
            return ReadJson();
        }
           
        return _appConfig;
    }
}

#region Class structure 
public partial class ConfigAppJson : IConfigAppJson
{
    private static readonly Lazy<ConfigAppJson> _lazyInstance = new(() => new ConfigAppJson());
    public static ConfigAppJson Instance => _lazyInstance.Value;
    private ConfigAppJson() { }
}
#endregion
