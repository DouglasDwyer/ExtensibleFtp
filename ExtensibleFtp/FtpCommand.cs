namespace DouglasDwyer.ExtensibleFtp
{
    /// <summary>
    /// Represents an FTP command that a user may issue to an FTP server to perform a certain action.
    /// </summary>
    public abstract class FtpCommand
    {
        /// <summary>
        /// The name of the command that the user should issue.
        /// </summary>
        public abstract string CommandName { get; }

        /// <summary>
        /// This method is called when a user issues this command.
        /// </summary>
        /// <param name="user">The user who made the request.</param>
        /// <param name="arguments">Any extra data that the user sent along with the name of the request.</param>
        public abstract void Execute(ExtensibleFtpUser user, string arguments);
    }
}