using DouglasDwyer.ExtensibleFtp.Commands;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DouglasDwyer.ExtensibleFtp
{
    public sealed class ExtensibleFtpServer
    {
        public static readonly List<FtpCommand> DefaultCommandSet = new List<FtpCommand>()
        {
            new CwdCommand(),
            new DeleCommand(),
            new ListCommand(),
            new MkdCommand(),
            new NoOpCommand(),
            new PassCommand(),
            new PasvCommand(),
            new PortCommand(),
            new PwdCommand(),
            new QuitCommand(),
            new RetrCommand(),
            new RmdaCommand(),
            new RmdCommand(),
            new RnfrCommand(),
            new RntoCommand(),
            new SizeCommand(),
            new StorCommand(),
            new TypeCommand(),
            new UserCommand()
        };

        public IFtpAuthenticator Authenticator { get; }
        public IList<FtpCommand> CommandSet => IndexedCommandSet.Values.ToImmutableList();
        public IList<ExtensibleFtpUser> OnlineUsers => ConnectedUsers.ToImmutableList();
        
        private Dictionary<string, FtpCommand> IndexedCommandSet = new Dictionary<string, FtpCommand>();
        private SynchronizedCollection<ExtensibleFtpUser> ConnectedUsers = new SynchronizedCollection<ExtensibleFtpUser>();
        private TcpListener ControlServer;
        private object Locker = new object();

        public ExtensibleFtpServer(IFtpAuthenticator authenticator)
        {
            Authenticator = authenticator;
            IndexedCommandSet = new Dictionary<string,FtpCommand>(DefaultCommandSet.Select(x => new KeyValuePair<string, FtpCommand>(x.CommandName.ToLowerInvariant(), x)));
        }

        public ExtensibleFtpServer(IFtpAuthenticator authenticator, List<FtpCommand> commandSet)
        {
            Authenticator = authenticator;
            IndexedCommandSet = new Dictionary<string, FtpCommand>(commandSet.Select(x => new KeyValuePair<string, FtpCommand>(x.CommandName.ToLowerInvariant(), x)));
        }

        public async Task StartAsync(int port)
        {
            lock(Locker)
            {
                if(ControlServer != null)
                {
                    throw new InvalidOperationException("The server is already running.");
                }
                ControlServer = new TcpListener(IPAddress.Any, port);
            }
            ControlServer.Start();
            await AcceptNewClientsAsync();
        }

        public void Stop()
        {
            lock (Locker)
            {
                ControlServer.Stop();
                foreach (ExtensibleFtpUser user in ConnectedUsers.ToArray())
                {
                    try
                    {
                        user.Stop();
                    }
                    catch { }
                }
                ControlServer = null;
            }
        }

        public FtpCommand GetCommand(string commandName)
        {
            FtpCommand toReturn = null;
            IndexedCommandSet.TryGetValue(commandName.ToLowerInvariant(), out toReturn);
            return toReturn;
        }

        internal void TryRemoveUser(ExtensibleFtpUser user)
        {
            ConnectedUsers.Remove(user);
        }

        private async Task AcceptNewClientsAsync()
        {
            while(ControlServer != null)
            {
                ExtensibleFtpUser user = new ExtensibleFtpUser(this, await ControlServer.AcceptTcpClientAsync());
                ConnectedUsers.Add(user);
                user.Start();
            }
        }
    }
}
