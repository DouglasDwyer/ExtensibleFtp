namespace DouglasDwyer.ExtensibleFtp.Anonymous
{
    public class AnonymousIdentity : IFtpIdentity
    {
        public IFtpFilesystem Filesystem { get; set; }

        public AnonymousIdentity(string rootDirectory)
        {
            Filesystem = new AnonymousFilesystem(rootDirectory);
        }
    }
}