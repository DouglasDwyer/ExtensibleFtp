using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace DouglasDwyer.ExtensibleFtp.Commands
{
    public class RmdCommand : FtpCommand
    {
        public override string CommandName => "RMD";

        public override void Execute(ExtensibleFtpUser user, string path)
        {
            path = Path.Combine(user.CurrentDirectory, path);
            if(user.Filesystem.DirectoryExists(path))
            {
                if(user.Filesystem.GetFiles(path).Length > 0)
                {
                    throw new FtpException(FtpStatusCode.FileActionAborted, "Directory is not empty.");
                }
                else
                {
                    user.Filesystem.DeleteDirectory(path);
                    user.SendResponse(FtpStatusCode.FileActionOK, "Directory deleted successfully.");
                }
            }
            else
            {
                throw new FtpException(FtpStatusCode.FileActionAborted, "Directory does not exist.");
            }
        }
    }
}
