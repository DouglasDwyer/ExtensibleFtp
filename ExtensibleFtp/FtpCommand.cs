namespace DouglasDwyer.ExtensibleFtp
{
    public abstract class FtpCommand
    {
        public abstract string CommandName { get; }

        public abstract void Execute(ExtensibleFtpUser user, string arguments);
    }
}