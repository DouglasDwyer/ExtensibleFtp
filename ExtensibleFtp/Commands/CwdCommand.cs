using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace DouglasDwyer.ExtensibleFtp.Commands
{
    public class CwdCommand : FtpCommand
    {
        public override string CommandName => "CWD";

        public override void Execute(ExtensibleFtpUser user, string arguments)
        {
            if (!arguments.StartsWith("/"))
            {
                arguments = SimplifyPath(Path.Combine(user.CurrentDirectory, arguments).Replace("\\", "/"));
            }
            if (user.Filesystem.DirectoryExists(arguments))
            {
                user.CurrentDirectory = arguments;
                user.SendResponse(FtpStatusCode.FileActionOK, "Changed to new directory.");
            }
            else
            {
                throw new FtpException(FtpStatusCode.ActionNotTakenFileUnavailable, "Directory not found.");
            }
        }

        private static string SimplifyPath(string path)
        {
            string[] pathSections = path.Split("/", StringSplitOptions.RemoveEmptyEntries);
            Stack<string> pathStack = new Stack<string>();
            for(int x = 0; x < pathSections.Length; x++)
            {
                if(pathSections[x] == "..")
                {
                    pathStack.Pop();
                }
                else
                {
                    pathStack.Push("/" + pathSections[x]);
                }
            }
            return pathStack.Count == 0 ? "/" : string.Concat(pathStack.Reverse());
        }
    }
}
