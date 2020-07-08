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
    /// <summary>
    /// Acts as a server which facilitates interaction between remote users and the host filesystem through the FTP protocol.
    /// </summary>
    public sealed class ExtensibleFtpServer
    {
        /// <summary>
        /// The default list of FTP commands to use when instantiating new instances of <see cref="ExtensibleFtpServer"/>.  It contains all the basic commands necessary for FTP transactions.
        /// </summary>
        public static List<FtpCommand> DefaultCommandSet = new List<FtpCommand>()
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
        /// <summary>
        /// The object which manages the authentication of users attempting to log into the FTP system.
        /// </summary>
        public IFtpAuthenticator Authenticator { get; }
        /// <summary>
        /// The command set that this FTP server is currently using.
        /// </summary>
        public IList<FtpCommand> CommandSet => IndexedCommandSet.Values.ToImmutableList();
        /// <summary>
        /// A list of users currently logged into the server.
        /// </summary>
        public IList<ExtensibleFtpUser> OnlineUsers => ConnectedUsers.ToImmutableList();
        
        private Dictionary<string, FtpCommand> IndexedCommandSet = new Dictionary<string, FtpCommand>();
        private SynchronizedCollection<ExtensibleFtpUser> ConnectedUsers = new SynchronizedCollection<ExtensibleFtpUser>();
        private TcpListener ControlServer;
        private object Locker = new object();
        
        /// <summary>
        /// Creates a new <see cref="ExtensibleFtpServer"/> instance with the specified authenticator.
        /// </summary>
        /// <param name="authenticator">An FTP authenticator which manages user identities.</param>
        public ExtensibleFtpServer(IFtpAuthenticator authenticator)
        {
            Authenticator = authenticator;
            IndexedCommandSet = new Dictionary<string,FtpCommand>(DefaultCommandSet.Select(x => new KeyValuePair<string, FtpCommand>(x.CommandName.ToLowerInvariant(), x)));
        }

        /// <summary>
        /// Creates a new <see cref="ExtensibleFtpServer"/> instance with the specified authenticator and command set.
        /// </summary>
        /// <param name="authenticator">An FTP authenticator which manages user identities.</param>
        /// <param name="commandSet">The set of FTP commands to use.</param>
        public ExtensibleFtpServer(IFtpAuthenticator authenticator, List<FtpCommand> commandSet)
        {
            Authenticator = authenticator;
            IndexedCommandSet = new Dictionary<string, FtpCommand>(commandSet.Select(x => new KeyValuePair<string, FtpCommand>(x.CommandName.ToLowerInvariant(), x)));
        }

        /// <summary>
        /// Starts the FTP server.
        /// </summary>
        /// <param name="port">The port that the FTP server should listen on.</param>
        public void Start(int port = 21)
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
            Task.Run(AcceptNewClientsAsync);
        }

        /// <summary>
        /// Stops the FTP server, severing all client connections.
        /// </summary>
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

        /// <summary>
        /// Gets a registered FTP command by the command's name.
        /// </summary>
        /// <param name="commandName">The name of the command in the current command set to obtain.</param>
        /// <returns>The specified command, or null if no command was found.</returns>
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
