namespace DouglasDwyer.ExtensibleFtp.Anonymous
{
    /// <summary>
    /// Represents the identity of an anonymous user who has complete control over any files in a certain root directory.
    /// </summary>
    public class AnonymousIdentity : IFtpIdentity
    {
        /// <summary>
        /// The filesystem that this user has access to.
        /// </summary>
        public IFtpFilesystem Filesystem { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="AnonymousIdentity"/> with access to the specified directory and its subdirectories.
        /// </summary>
        /// <param name="rootDirectory">The root directory this user should have access to.</param>
        public AnonymousIdentity(string rootDirectory)
        {
            Filesystem = new AnonymousFilesystem(rootDirectory);
        }
    }
}