namespace DouglasDwyer.ExtensibleFtp
{
    public interface IFtpIdentity
    {
        public IFtpFilesystem Filesystem { get; }
    }
}