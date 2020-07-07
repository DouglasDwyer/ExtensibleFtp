using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace DouglasDwyer.ExtensibleFtp.Commands
{
    public class MkdCommand : FtpCommand
    {
        public override string CommandName => "MKD";

        public override void Execute(ExtensibleFtpUser user, string path)
        {
            user.Filesystem.CreateDirectory(Path.Combine(user.CurrentDirectory, path));
            user.SendResponse(FtpStatusCode.PathnameCreated, "Directory created successfully.");
        }
    }
}
