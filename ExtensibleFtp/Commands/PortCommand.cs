using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DouglasDwyer.ExtensibleFtp.Commands
{
    public class PortCommand : FtpCommand
    {
        public override string CommandName => "PORT";

        public override void Execute(ExtensibleFtpUser user, string arguments)
        {
            string[] portArguments = arguments.Split(",");
            byte[] portData = new byte[portArguments.Length];
            for(int x = 0; x < portArguments.Length; x++)
            {
                portData[x] = byte.Parse(portArguments[x]);
            }
            if (BitConverter.IsLittleEndian)
            {
                byte portData5 = portData[5];
                portData[5] = portData[4];
                portData[4] = portData5;
            }
            user.DataClient?.Dispose();
            user.DataClient = new ActiveDataConnector(new IPEndPoint(new IPAddress(portData[0..3]), BitConverter.ToInt16(portData[4..5])));
            user.SendResponse(FtpStatusCode.CommandOK, "Entered active mode.");
        }
    }
}
