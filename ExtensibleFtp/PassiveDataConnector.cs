using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DouglasDwyer.ExtensibleFtp
{
    public class PassiveDataConnector : IDataConnector
    {
        public TcpListener DataListener = new TcpListener(IPAddress.Any, 0);

        public PassiveDataConnector()
        {
            DataListener.Start();
        }

        public async Task<TcpClient> CreateDataConnectionAsync()
        {
            return await DataListener.AcceptTcpClientAsync();
        }

        public void Dispose()
        {
            DataListener.Stop();
        }
    }
}
