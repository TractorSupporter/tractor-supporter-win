using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TractorSupporter.Interfaces
{
    public interface IDataReceiver
    {
        Byte[] ReceiveData();
        String GetRemoteIpAddress();
    }
}
