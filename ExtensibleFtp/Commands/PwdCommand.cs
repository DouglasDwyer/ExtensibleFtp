using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DouglasDwyer.ExtensibleFtp.Commands
{
    public class PwdCommand : FtpCommand
    {
        public override string CommandName => "PWD";

        public override void Execute(ExtensibleFtpUser user, string arguments)
        {
            user.SendResponse(FtpStatusCode.PathnameCreated, "\"" + user.CurrentDirectory + "\" is current directory.");
        }
    }
}
