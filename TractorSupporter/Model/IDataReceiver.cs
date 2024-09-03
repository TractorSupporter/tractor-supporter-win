using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TractorSupporter.Model
{
    public interface IDataReceiver
    {
        byte[] ReceiveData();
        string GetRemoteIpAddress();
    }
}
