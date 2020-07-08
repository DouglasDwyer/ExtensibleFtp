using System;
using System.Collections.Generic;
using System.Text;

namespace DouglasDwyer.ExtensibleFtp.Anonymous
{
    /// <summary>
    /// Represents an authenticator which allows users to connect to the FTP server under any username.
    /// </summary>
    public class AnonymousAuthenticator : IFtpAuthenticator
    {
        /// <summary>
        /// The root directory from which users can edit files.
        /// </summary>
        public string RootDirectory { get; set; }

        /// <summary>
        /// Creates a new <see cref="AnonymousAuthenticator"/> instance.
        /// </summary>
        public AnonymousAuthenticator()
        {

        }

        /// <summary>
        /// Creates a new <see cref="AnonymousAuthenticator"/> instance with the specified data.
        /// </summary>
        /// <param name="rootDirectory">The root directory under which users can edit files.</param>
        public AnonymousAuthenticator(string rootDirectory)
        {
            RootDirectory = rootDirectory + "/";
        }

        /// <summary>
        /// Authenticates a user, identifying them from their username/password.
        /// </summary>
        /// <param name="username">The username of the individual attempting to log in.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns>An <see cref="IFtpIdentity"/> object representing the logged-in user, or <c>null</c> if the attempt failed.</returns>
        public IFtpIdentity AuthenticateUser(string username, string password)
        {
            return new AnonymousIdentity(RootDirectory);
        }
    }
}
