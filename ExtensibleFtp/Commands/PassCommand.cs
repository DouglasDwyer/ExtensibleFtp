using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DouglasDwyer.ExtensibleFtp.Commands
{
    public class PassCommand : FtpCommand
    {
        public override string CommandName => "PASS";

        public override void Execute(ExtensibleFtpUser user, string arguments)
        {
            IFtpIdentity identity = user.Host.Authenticator.AuthenticateUser((string)user.LastCommandData, arguments);
            if(identity is null)
            {
                throw new FtpException(FtpStatusCode.NotLoggedIn, "Not logged in.");
            }
            else
            {
                user.Identity = identity;
                user.SendResponse(FtpStatusCode.LoggedInProceed, "User logged in.");
            }
        }
    }
}
