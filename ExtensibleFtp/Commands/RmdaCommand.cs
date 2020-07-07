using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace DouglasDwyer.ExtensibleFtp.Commands
{
    public class RmdaCommand : FtpCommand
    {
        public override string CommandName => "RMDA";

        public override void Execute(ExtensibleFtpUser user, string path)
        {
            path = Path.Combine(user.CurrentDirectory, path);
            if (user.Filesystem.DirectoryExists(path))
            {
                user.Filesystem.DeleteDirectory(path);
                user.SendResponse(FtpStatusCode.FileActionOK, "Directory deleted successfully.");
            }
            else
            {
                throw new FtpException(FtpStatusCode.FileActionAborted, "Directory does not exist.");
            }
        }
    }
}
