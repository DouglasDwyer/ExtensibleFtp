using System;
using System.Collections.Generic;
using System.Text;

namespace DouglasDwyer.ExtensibleFtp.Commands
{
    public class UserCommand : FtpCommand
    {
        public override string CommandName => "USER";

        public override void Execute(ExtensibleFtpUser user, string arguments)
        {
            user.LastCommandData = arguments;
            user.SendResponse(331, "Username ok, need password");
        }
    }
}
