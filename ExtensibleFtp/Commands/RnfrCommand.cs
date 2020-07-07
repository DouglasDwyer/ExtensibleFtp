using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace DouglasDwyer.ExtensibleFtp.Commands
{
    public class RnfrCommand : FtpCommand
    {
        public override string CommandName => "RNFR";

        public override void Execute(ExtensibleFtpUser user, string path)
        {
            path = Path.Combine(user.CurrentDirectory, path);
            if (user.Filesystem.FileExists(path) || user.Filesystem.DirectoryExists(path))
            {
                user.LastCommandData = path;
                user.SendResponse(FtpStatusCode.FileCommandPending, "Item selected.  New item name needed.");
            }
            else
            {
                throw new FtpException(FtpStatusCode.ActionNotTakenFileUnavailable, "No item exists under that name.");
            }
        }
    }
}
