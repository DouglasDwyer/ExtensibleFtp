using System;
using System.Collections.Generic;
using System.Text;

namespace DouglasDwyer.ExtensibleFtp
{
    /// <summary>
    /// Represents an object which manages user accounts and has the ability to authenticate users given their username and password.
    /// </summary>
    public interface IFtpAuthenticator
    {
        /// <summary>
        /// Authenticates a user, identifying them from their username/password.
        /// </summary>
        /// <param name="username">The username of the individual attempting to log in.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns>An <see cref="IFtpIdentity"/> object representing the logged-in user, or <c>null</c> if the attempt failed.</returns>
        public IFtpIdentity AuthenticateUser(string username, string password);
    }
}
