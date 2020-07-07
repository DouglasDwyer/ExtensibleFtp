using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DouglasDwyer.ExtensibleFtp
{
    public class ActiveDataConnector : IDataConnector
    {
        public IPEndPoint ConnectionEndpoint { get; }
        private TcpClient InternalClient;

        public ActiveDataConnector(IPEndPoint connectionEndpoint)
        {
            ConnectionEndpoint = connectionEndpoint;
        }

        public async Task<TcpClient> CreateDataConnectionAsync()
        {
            InternalClient = new TcpClient();
            await InternalClient.ConnectAsync(ConnectionEndpoint.Address, ConnectionEndpoint.Port);
            return InternalClient;
        }

        public void Dispose()
        {
            InternalClient.Dispose();
        }
    }
}
