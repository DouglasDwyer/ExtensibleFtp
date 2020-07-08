using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace DouglasDwyer.ExtensibleFtp
{
    /// <summary>
    /// Represents a factory used for generating data connections.
    /// </summary>
    public interface IDataConnector : IDisposable
    {
        /// <summary>
        /// Creates a new data connection.
        /// </summary>
        /// <returns>A <see cref="TcpClient"/> that can be used to transfer data.  The client should be disposed when no longer in use.</returns>
        public Task<TcpClient> CreateDataConnectionAsync();
    }
}