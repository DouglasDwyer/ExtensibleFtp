using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DouglasDwyer.ExtensibleFtp
{
    /// <summary>
    /// Represents an "active" data connector, which creates data connections by connecting to a port on clients' machines.
    /// </summary>
    public class ActiveDataConnector : IDataConnector
    {
        /// <summary>
        /// The endpoint to connect to when creating data connections.
        /// </summary>
        public IPEndPoint ConnectionEndpoint { get; }
        private TcpClient InternalClient;

        /// <summary>
        /// Creates a new active data connector with the specified endpoint.
        /// </summary>
        /// <param name="connectionEndpoint">The endpoint that should be connected to when creating data connections.</param>
        public ActiveDataConnector(IPEndPoint connectionEndpoint)
        {
            ConnectionEndpoint = connectionEndpoint;
        }

        /// <summary>
        /// Creates a new data connection.
        /// </summary>
        /// <returns>A <see cref="TcpClient"/> that can be used to transfer data.  The client should be disposed when no longer in use.</returns>
        public async Task<TcpClient> CreateDataConnectionAsync()
        {
            InternalClient = new TcpClient();
            await InternalClient.ConnectAsync(ConnectionEndpoint.Address, ConnectionEndpoint.Port);
            return InternalClient;
        }

        /// <summary>
        /// Disposes the connector, closing any active data connections in the process.
        /// </summary>
        public void Dispose()
        {
            InternalClient.Dispose();
        }
    }
}
