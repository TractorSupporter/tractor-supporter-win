using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TractorSupporter.Model.Enums;

namespace TractorSupporter.Model
{
    public class AppConfig
    {
        public int Port { get; set; }
        public string IpAddress { get; set; }
        public bool Option1 { get; set; }
        public bool Option2 { get; set; }
        public TypeSensor SelectedSensorType { get; set; }
    }
}
