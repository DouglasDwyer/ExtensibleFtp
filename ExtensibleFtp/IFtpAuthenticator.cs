using System;
using System.Collections.Generic;
using System.Text;

namespace DouglasDwyer.ExtensibleFtp
{
    public interface IFtpAuthenticator
    {
        public IFtpIdentity AuthenticateUser(string username, string password);
    }
}
