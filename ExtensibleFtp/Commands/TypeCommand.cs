using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DouglasDwyer.ExtensibleFtp.Commands
{
    public class TypeCommand : FtpCommand
    {
        public override string CommandName => "TYPE";

        public override void Execute(ExtensibleFtpUser user, string arguments)
        {
            string[] splitArgs = arguments.Split(' ');
            string mode = splitArgs[0];
            string format = splitArgs.Length > 1 ? splitArgs[1] : null;
            switch (mode)
            {
                case "A":
                    user.TransferType = TransferMode.ASCII;
                    break;
                case "I":
                    user.TransferType = TransferMode.Binary;
                    break;
                case "E":
                case "L":
                default:
                    throw new FtpException(FtpStatusCode.CommandNotImplemented, "Command not implemented for that parameter.");
            }
            if (format != null)
            {
                switch (format)
                {
                    case "N":
                        break;
                    case "T":
                    case "C":
                    default:
                        throw new FtpException(FtpStatusCode.CommandNotImplemented, "Command not implemented for that parameter.");
                }
            }
            user.SendResponse(FtpStatusCode.CommandOK, "OK");
        }
    }
}
