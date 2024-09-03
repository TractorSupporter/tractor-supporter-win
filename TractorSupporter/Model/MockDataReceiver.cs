using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TractorSupporter.Model
{
    public class MockDataReceiver : IDataReceiver
    {
        public static string ExtraMessage { get; set; } = "extra message";
        public static double DistanceMeasured { get; set; } = 1000;


        public byte[] ReceiveData()
        {
            var mockData = new
            {
                extraMessage = ExtraMessage,
                distanceMeasured = DistanceMeasured
            };

            string jsonString = JsonSerializer.Serialize(mockData);
            return Encoding.ASCII.GetBytes(jsonString);
        }

        public string GetRemoteIpAddress()
        {
            return "0.0.0.0";
        }
    }
}
