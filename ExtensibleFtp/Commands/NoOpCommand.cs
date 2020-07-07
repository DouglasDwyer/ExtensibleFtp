using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DouglasDwyer.ExtensibleFtp.Commands
{
    public class NoOpCommand : FtpCommand
    {
        public override string CommandName => "NOOP";

        public override void Execute(ExtensibleFtpUser user, string arguments)
        {
            user.SendResponse(FtpStatusCode.CommandOK, "Service OK.");
        }
    }
}
