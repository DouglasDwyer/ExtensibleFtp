using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DouglasDwyer.ExtensibleFtp.Commands
{
    public sealed class QuitCommand : FtpCommand
    {
        public override string CommandName => "QUIT";

        public override void Execute(ExtensibleFtpUser user, string arguments)
        {
            user.SendResponse(FtpStatusCode.ClosingControl, "Service closing control connection.");
        }
    }
}
