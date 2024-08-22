﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TractorSupporter.Interfaces;

namespace TractorSupporter.Services
{
    public class MockDataReceiver : IDataReceiver
    {
        public static string ExtraMessage { get; set; } = "extra message";
        public static double DistanceMeasured { get; set; } = 10;


        public Byte[] ReceiveData()
        {
            var mockData = new
            {
                extraMessage = ExtraMessage,
                distanceMeasured = DistanceMeasured
            };

            string jsonString = JsonSerializer.Serialize(mockData);
            return Encoding.ASCII.GetBytes(jsonString);
        }

        public String GetRemoteIpAddress()
        {
            return "0.0.0.0";
        }
    }
}
