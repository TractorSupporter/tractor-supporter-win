using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TractorSupporter.Model;
using TractorSupporter.Services.Interfaces;

namespace TractorSupporter.Services
{
    public class ConfigAppJson : IConfigAppJson
    {
        private static ConfigAppJson _instance;
        private static readonly object _lock = new object();
        private AppConfig _appConfig;

        readonly string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        readonly string fileName = "config.json";

        private ConfigAppJson() { }

        public static ConfigAppJson Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new ConfigAppJson();
                    }
                    return _instance;
                }
            }
        }

        public void createJson(string port, string ipAddress)
        {
            var config = new AppConfig
            {
                Port = int.Parse(port),
                IpAddress = ipAddress
            };

            string jsonString = JsonSerializer.Serialize(config);
            string filePath = Path.Combine(baseDirectory, fileName);

            File.WriteAllText(filePath, jsonString);
        }

        public AppConfig readJson()
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
            return _appConfig;
        }
    }
}
