using System;
using System.Collections.Generic;
using System.Text;

namespace DouglasDwyer.ExtensibleFtp.Anonymous
{
    public class AnonymousAuthenticator : IFtpAuthenticator
    {
        public string RootDirectory { get; set; }

        public AnonymousAuthenticator()
        {

        }

        public AnonymousAuthenticator(string rootDirectory)
        {
            RootDirectory = rootDirectory;
        }

        public IFtpIdentity AuthenticateUser(string username, string password)
        {
            return new AnonymousIdentity(RootDirectory);
        }
    }
}
