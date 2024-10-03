using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TractorSupporter.Model;

namespace TractorSupporter.Services.Interfaces
{
    public interface IConfigAppJson
    {
        public void CreateJson(string port, string ipAddress);
        public AppConfig ReadJson();
    }
}
