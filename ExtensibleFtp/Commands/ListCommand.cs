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
    public class ListCommand : FtpCommand
    {
        public override string CommandName => "LIST";

        public override void Execute(ExtensibleFtpUser user, string pathname)
        {
            if (pathname == null)
            {
                pathname = "";
            }

            pathname = Path.Combine(user.CurrentDirectory, pathname);
            if(user.Filesystem.DirectoryExists(pathname))
            {
                ExecuteAsync(user, pathname);
                user.SendResponse(FtpStatusCode.OpeningData, $"Opening {user.DataClient} mode data transfer for LIST");
            }
            else
            {
                throw new FtpException(FtpStatusCode.ActionNotTakenFileUnavailable, "Requested file action not taken.");
            }
        }

        private async Task ExecuteAsync(ExtensibleFtpUser user, string pathname)
        {
            using (TcpClient client = await user.DataClient.CreateDataConnectionAsync())
            using (StreamWriter writer = new StreamWriter(client.GetStream(), Encoding.ASCII))
            {
                foreach(string directory in user.Filesystem.GetSubdirectories(pathname))
                {
                    DirectoryInfo d = user.Filesystem.GetDirectoryInfo(directory);

                    string date = d.LastWriteTime < DateTime.Now - TimeSpan.FromDays(180) ?
                        d.LastWriteTime.ToString("MMM dd  yyyy") :
                        d.LastWriteTime.ToString("MMM dd HH:mm");

                    writer.WriteLine($"{user.Filesystem.GetDirectoryPermissions(directory)} 1 IMS IMS 0 {date} {d.Name}");
                    writer.Flush();
                }
                foreach (string file in user.Filesystem.GetFiles(pathname))
                {
                    FileInfo f = user.Filesystem.GetFileInfo(file);

                    string date = f.LastWriteTime < DateTime.Now - TimeSpan.FromDays(180) ?
                        f.LastWriteTime.ToString("MMM dd  yyyy") :
                        f.LastWriteTime.ToString("MMM dd HH:mm");

                    writer.WriteLine($"{user.Filesystem.GetFilePermissions(file)} 1 IMS IMS {f.Length} {date} {f.Name}");
                    writer.Flush();
                }
            }
            user.SendResponse(FtpStatusCode.ClosingData, "Transfer complete.");
        }
    }
}
