using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TractorSupporter.Model;
using TractorSupporter.Model.Enums;

namespace TractorSupporter.Services.Interfaces
{
    public interface IConfigAppJson
    {
        public void CreateJson(string port, string ipAddress, bool isAvoidingMechanismTurnedOn, bool isAlarmMechanismTurnedOn, TypeSensor selectedSensorType, int AvoidingDistance, int AlarmDistance, Language language);
        public AppConfig ReadJson();
        public AppConfig GetConfig();
    }
}
