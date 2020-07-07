using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace DouglasDwyer.ExtensibleFtp
{
    public class DataConnection : IDisposable
    {
        public TcpClient DataClient;

        public virtual void Dispose()
        {
            DataClient.Close();
            DataClient.Dispose();
        }
    }
}
