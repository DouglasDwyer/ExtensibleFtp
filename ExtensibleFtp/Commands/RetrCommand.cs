using DouglasDwyer.ExtensibleFtp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DouglasDwyer.ExtensibleFtp.Commands
{
    public class RetrCommand : FtpCommand
    {
        public override string CommandName => "RETR";

        public override void Execute(ExtensibleFtpUser user, string arguments)
        {
            arguments = Path.Combine(user.CurrentDirectory, arguments);
            if(user.Filesystem.FileExists(arguments))
            {
                ExecuteAsync(user, arguments);
                user.SendResponse(FtpStatusCode.OpeningData, $"Opening {user.DataClient} mode data transfer for RETR");
            }
            else
            {
                throw new FtpException(FtpStatusCode.ActionNotTakenFileUnavailable, "Requested file action not taken.");
            }
        }

        private async Task ExecuteAsync(ExtensibleFtpUser user, string path)
        {
            using TcpClient client = await user.DataClient.CreateDataConnectionAsync();
            using FileStream stream = user.Filesystem.GetFileStream(path);
            if (user.TransferType == TransferMode.ASCII)
            {
                await CopyStreamAsciiAsync(stream, client.GetStream(), 81920);
                user.SendResponse(FtpStatusCode.ClosingData, "Closing data connection, file transfer successful.");
            }
            else if (user.TransferType == TransferMode.Binary)
            {
                await stream.CopyToAsync(client.GetStream());
                user.SendResponse(FtpStatusCode.ClosingData, "Closing data connection, file transfer successful.");
            }
            else
            {
                throw new FtpException(FtpStatusCode.CommandNotImplemented, "Unsupported transfer mode.");
            }
        }

        private static async Task CopyStreamAsciiAsync(Stream input, Stream output, int bufferSize)
        {
            char[] buffer = new char[bufferSize];
            int count = 0;

            using (StreamReader rdr = new StreamReader(input))
            {
                using (StreamWriter wtr = new StreamWriter(output, Encoding.ASCII))
                {
                    while ((count = rdr.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        await wtr.WriteAsync(buffer, 0, count);
                    }
                }
            }
        }
    }
}
