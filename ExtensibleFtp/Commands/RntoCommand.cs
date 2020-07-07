using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace DouglasDwyer.ExtensibleFtp.Commands
{
    public class RntoCommand : FtpCommand
    {
        public override string CommandName => "RNTO";

        public override void Execute(ExtensibleFtpUser user, string newPath)
        {
            string oldPath = (string)user.LastCommandData;
            newPath = Path.Combine(user.CurrentDirectory, newPath);
            if(user.Filesystem.FileExists(oldPath))
            {
                user.Filesystem.MoveFile(oldPath, newPath);
            }
            else
            {
                user.Filesystem.MoveDirectory(oldPath, newPath);
            }
            user.SendResponse(FtpStatusCode.FileActionOK, "Item moved successfully.");
        }
    }
}
