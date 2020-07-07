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
    public sealed class ExtensibleFtpUser
    {
        public ExtensibleFtpServer Host { get; }
        public IFtpIdentity Identity { get; set; }
        public IFtpFilesystem Filesystem => Identity.Filesystem;
        public TransferMode TransferType { get; set; } = TransferMode.ASCII;
        public IDataConnector DataClient { get; set; }

        public string CurrentDirectory { get; set; } = "/";

        public object LastCommandData;

        private CancellationTokenSource ListeningCancellationSource;
        public TcpClient ControlClient { get; private set; }
        private NetworkStream ControlClientStream => ControlClient.GetStream();
        private StreamWriter ControlWriter;
        private StreamReader ControlReader;

        public ExtensibleFtpUser(ExtensibleFtpServer host, TcpClient client)
        {
            Host = host;
            ControlClient = client;
        }

        public void Start()
        {
            ListeningCancellationSource = new CancellationTokenSource();
            Listen(ListeningCancellationSource.Token);
        }

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
                        SendResponse(e.HttpCode, e.Message);
                    }
                    catch(Exception)
                    {
                        SendResponse(FtpStatusCode.ActionAbortedLocalProcessingError, "Internal server error.");
                    }
                }
            }
        }

        public void SendResponse(FtpStatusCode code, string data)
        {
            SendResponse((int)code, data);
        }

        public void SendResponse(int code, string data)
        {
            ControlWriter.WriteLine(code + " " + data);
            ControlWriter.Flush();
        }
    }   
}
