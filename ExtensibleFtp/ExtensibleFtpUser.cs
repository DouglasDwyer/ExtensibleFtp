using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace DouglasDwyer.ExtensibleFtp
{
    /// <summary>
    /// Represents a client that is currently connected to an <see cref="ExtensibleFtpServer"/>.
    /// </summary>
    public sealed class ExtensibleFtpUser
    {
        /// <summary>
        /// The FTP server that is hosting this client.
        /// </summary>
        public ExtensibleFtpServer Host { get; }
        /// <summary>
        /// The client's user identity, or null if the client is not logged in.
        /// </summary>
        public IFtpIdentity Identity { get; set; }
        /// <summary>
        /// The filesystem associated with the current <see cref="Identity"/>.  This may throw an exception if the user is not logged in.
        /// </summary>
        public IFtpFilesystem Filesystem => Identity.Filesystem;
        /// <summary>
        /// The encoding type to use when transferring information across the data connection.
        /// </summary>
        public TransferMode TransferType { get; set; } = TransferMode.ASCII;
        /// <summary>
        /// The current data connector.
        /// </summary>
        public IDataConnector DataClient { get; set; }
        /// <summary>
        /// Whether the client has been authenticated.
        /// </summary>
        public bool IsLoggedIn => Identity != null;
        /// <summary>
        /// The user's current FTP directory.
        /// </summary>
        public string CurrentDirectory { get; set; } = "/";

        /// <summary>
        /// An object which contains data put there by the last command used.  Two-part commands, like USER and PASS, use this object to pass data between each other.
        /// </summary>
        public object LastCommandData;

        private CancellationTokenSource ListeningCancellationSource;
        public TcpClient ControlClient { get; private set; }
        private NetworkStream ControlClientStream => ControlClient.GetStream();
        private StreamWriter ControlWriter;
        private StreamReader ControlReader;

        internal ExtensibleFtpUser(ExtensibleFtpServer host, TcpClient client)
        {
            Host = host;
            ControlClient = client;
        }

        internal void Start()
        {
            ListeningCancellationSource = new CancellationTokenSource();
            Listen(ListeningCancellationSource.Token);
        }

        /// <summary>
        /// Stops the FTP connection and disconnects the user from the server.
        /// </summary>
        public void Stop()
        {
            ListeningCancellationSource.Cancel();
            Host.TryRemoveUser(this);
        }

        private async Task<string> GetNextLine(CancellationToken token)
        {
            Task<string> nextLine = ControlReader.ReadLineAsync();
            while (true)
            {
                if(token.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }
                else if (nextLine.IsCompleted)
                {
                    return nextLine.Result;
                }
                await Task.Delay(1);
            }
        }

        private async Task Listen(CancellationToken token)
        {
            using (ControlWriter = new StreamWriter(ControlClientStream, Encoding.ASCII))
            using (ControlReader = new StreamReader(ControlClientStream, Encoding.ASCII))
            {
                SendResponse(220, "Service ready for new user.");
                string line;
                while (!string.IsNullOrEmpty(line = await GetNextLine(token)))
                {
                    try
                    {
                        Match match = Regex.Match(line, @"^([a-zA-Z]*)(?: )?(.*)$");
                        if (match.Success)
                        {
                            FtpCommand command = Host.GetCommand(match.Groups[1].Value);
                            if(command is null)
                            {
                                throw new FtpException(502, "Command not implemented.");
                            }
                            else
                            {
                                string arguments = match.Groups.Count > 2 && !string.IsNullOrWhiteSpace(match.Groups[2].Value) ? match.Groups[2].Value : "";
                                command.Execute(this, arguments);
                            }
                        }
                        else
                        {
                            throw new FtpException(FtpStatusCode.ArgumentSyntaxError, "Syntax error in parameters or arguments.");
                        }
                    }
                    catch(FtpException e)
                    {
                        SendResponse(e.StatusCode, e.Message);
                    }
                    catch(Exception)
                    {
                        SendResponse(FtpStatusCode.ActionAbortedLocalProcessingError, "Internal server error.");
                    }
                }
            }
        }

        /// <summary>
        /// Sends an FTP response to the client.  FTP responses consist of a status code and a single-line message.
        /// </summary>
        /// <param name="code">The FTP status code to send.</param>
        /// <param name="data">The message to send along with the status code.</param>
        public void SendResponse(FtpStatusCode code, string data)
        {
            SendResponse((int)code, data);
        }

        /// <summary>
        /// Sends an FTP response to the client.  FTP responses consist of a status code and a single-line message.
        /// </summary>
        /// <param name="code">The FTP status code to send.</param>
        /// <param name="data">The message to send along with the status code.</param>
        public void SendResponse(int code, string data)
        {
            ControlWriter.WriteLine(code + " " + data);
            ControlWriter.Flush();
        }
    }   
}
