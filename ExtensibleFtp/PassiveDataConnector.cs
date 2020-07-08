using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DouglasDwyer.ExtensibleFtp
{
    /// <summary>
    /// Represents a "passive" data connector, which creates data connections by listening for clients on a specific server-side port.
    /// </summary>
    public class PassiveDataConnector : IDataConnector
    {
        /// <summary>
        /// The current <see cref="TcpListener"/> used to listen for incoming passive data connections.
        /// </summary>
        public TcpListener DataListener = new TcpListener(IPAddress.Any, 0);

        /// <summary>
        /// Creates a new passive data connector, starting a <see cref="TcpListener"/> to manage data connections in the process.
        /// </summary>
        public PassiveDataConnector()
        {
            DataListener.Start();
        }

        /// <summary>
        /// Creates a new data connection.
        /// </summary>
        /// <returns>A <see cref="TcpClient"/> that can be used to transfer data.  The client should be disposed when no longer in use.</returns>
        public async Task<TcpClient> CreateDataConnectionAsync()
        {
            return await DataListener.AcceptTcpClientAsync();
        }

        /// <summary>
        /// Disposes the connector, closing any active data connections in the process.
        /// </summary>
        public void Dispose()
        {
            DataListener.Stop();
        }
    }
}
