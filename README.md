![Nuget](https://img.shields.io/nuget/v/DouglasDwyer.ExtensibleFtp)

# ExtensibleFtp
ExtensibleFtp provides a customizable, scalable FTP server implementation in .NET.  It comes with a default implementation for anonymous FTP access, and allows for easy addition of new commands, abstract filesystems, and user identities/login schemes.
### Referencing ExtensibleFtp
ExtensibleFtp is available as a Nuget package called `DouglasDwyer.ExtensibleFtp` for inclusion in any .NET project.  To include it in your project, either download it using the package manager GUI in Visual Studio, or run the command `Install-Package DouglasDwyer.ExtensibleFtp` from the Nuget package manager console.
### Basic concepts
ExtensibleFtp servers are separated into several classes in order to allow for scalability and easy reconfiguration.  The `ExtensibleFtpServer` class listens for new FTP connections while managing currently connected clients.  It contains a list of `FtpCommand`, which are objects that represent unique actions the user can commit over FTP.  The behavior of FTP commands such as `USER` or `PASV` is defined by inheriting the `FtpCommand` abstract class.  In addition, the server object contains an `IFtpAuthenticator` object, which is in charge of managing user accounts.  The authenticator is used to implement login behavior (such as checking for a username and password in a database), and generating `IFtpIdentity` instances, which represent unique user identities and may be used to define various user permissions.  In turn, `IFtpIdentity` objects expose an `IFtpFilesystem` interface, which is an abstraction over a user filesystem.  Custom `IFtpFilesystem` implementations allow for much flexibility and creativity - files/folders can be hidden, restricted, or even "fabricated" without really existing on the host machine.  A complete API reference may be found [here](https://douglasdwyer.github.io/ExtensibleFtp/).
### Getting started
#### Creating a new server
Starting a new FTP server using C# is as simple as two lines of code!  After referencing `DouglasDwyer.ExtensibleFtp`, one can instantiate FTP servers like this:
```c#
ExtensibleFtpServer server = new ExtensibleFtpServer(new AnonymousAuthenticator("C:/Some/Path/"));
server.Start();
```
This code creates a new server coupled with an `AnonymousAuthenticator`, an authenticator that allows any connected users access to the specified directory and all of its contents.  `server.Start()` causes the server to begin listening (on port 21 by default).
#### Adding a custom FTP command
A number of standard FTP commands are included in the ExtensibleFtp implementation.  If the need arises to create a new/custom command, though, the implementation is simple - just override the `FtpCommand` interface and specify the custom behavior.  The code for the `DELE` command is given below:
```c#
public class DeleCommand : FtpCommand
{
    public override string CommandName => "DELE";

    public override void Execute(ExtensibleFtpUser user, string path)
    {
        path = Path.Combine(user.CurrentDirectory, path);
        if(user.Filesystem.FileExists(path))
        {
            user.Filesystem.DeleteFile(path);
            user.SendResponse(FtpStatusCode.FileActionOK, "File deleted successfully.");
        }
        else
        {
            throw new FtpException(FtpStatusCode.ActionNotTakenFileUnavailable, "File does not exist.");
        }
    }
}
```
Instances of the `ExtensibleFtpUser` class are used to interact with clients.  It is separate from `IFtpIdentity` - the user class allows for actions like sending and receiving FTP messages, whereas an identity identifies a user.  Anyhow, to use a custom command with a server, it can either be passed into the `ExtensibleFtpServer` constructor along with all other commands that the server should use, or it can be added to the `ExtensibleFtpServer.DefaultCommandSet` list.  Any servers instantiated without a specified command set after the command is added to the list will recognize it.
