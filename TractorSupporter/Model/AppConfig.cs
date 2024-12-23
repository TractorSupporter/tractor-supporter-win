using TractorSupporter.Model.Enums;

namespace TractorSupporter.Model;

public class AppConfig
{
    public int Port { get; set; }
    public string IpAddress { get; set; }
    public bool IsAvoidingMechanismTurnedOn { get; set; }
    public bool IsAlarmMechanismTurnedOn { get; set; }
    public TypeSensor SelectedSensorType { get; set; }
    public TypeTurn SelectedTurnDirection { get; set; }
    public int AvoidingDistance { get; set; }
    public int AlarmDistance { get; set; }
    public Language Language { get; set; }
    public double VehicleWidth { get; set; } = 3.01; // meters
}
