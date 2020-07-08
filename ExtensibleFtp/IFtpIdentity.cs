namespace DouglasDwyer.ExtensibleFtp
{
    /// <summary>
    /// Represents the identity of an <see cref="ExtensibleFtpUser"/>.
    /// </summary>
    public interface IFtpIdentity
    {
        /// <summary>
        /// The filesystem that users with this identity should be allowed to interact with.
        /// </summary>
        public IFtpFilesystem Filesystem { get; }
    }
}