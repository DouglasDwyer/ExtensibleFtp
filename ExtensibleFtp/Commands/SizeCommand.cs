using DouglasDwyer.ExtensibleFtp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace DouglasDwyer.ExtensibleFtp.Commands
{
    public class SizeCommand : FtpCommand
    {
        public override string CommandName => "SIZE";

        public override void Execute(ExtensibleFtpUser user, string path)
        {
            path = Path.Combine(user.CurrentDirectory, path);
            if (user.Filesystem.FileExists(path))
            {
                user.SendResponse(FtpStatusCode.FileStatus, user.Filesystem.GetFileInfo(path).Length.ToString());
            }
            else
            {
                throw new FtpException(FtpStatusCode.ActionNotTakenFileUnavailable, "File not found.");
            }
        }
    }
}
