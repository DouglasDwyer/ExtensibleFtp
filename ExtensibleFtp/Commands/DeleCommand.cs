using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace DouglasDwyer.ExtensibleFtp.Commands
{
    public class DeleCommand : FtpCommand
    {
        public override string CommandName => "DELE";

        public override void Execute(ExtensibleFtpUser user, string path)
        {
            path = Path.Combine(user.CurrentDirectory, path);
            if(user.Filesystem.FileExists(path))
            {
                user.Filesystem.DeleteFile(path);
                user.SendResponse(FtpStatusCode.FileActionOK, "File deleted successfully.");
            }
            else
            {
                throw new FtpException(FtpStatusCode.ActionNotTakenFileUnavailable, "File does not exist.");
            }
        }
    }
}
