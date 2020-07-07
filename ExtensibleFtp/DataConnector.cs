using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace DouglasDwyer.ExtensibleFtp
{
    public interface IDataConnector : IDisposable
    {
        public Task<TcpClient> CreateDataConnectionAsync();
    }
}