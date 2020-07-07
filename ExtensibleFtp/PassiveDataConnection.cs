using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace DouglasDwyer.ExtensibleFtp
{
    public class PassiveDataConnection : DataConnection
    {
        public TcpListener DataListener;

        public override void Dispose()
        {
            base.Dispose();
            DataListener.Stop();
        }
    }
}
