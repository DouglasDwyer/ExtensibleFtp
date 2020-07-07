using DouglasDwyer.ExtensibleFtp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DouglasDwyer.ExtensibleFtp.Commands
{
    public class PasvCommand : FtpCommand
    {
        public override string CommandName => "PASV";

        public override void Execute(ExtensibleFtpUser user, string arguments)
        {
            user.DataClient?.Dispose();
            PassiveDataConnector connection = new PassiveDataConnector();
            user.DataClient = connection;
            byte[] address = ((IPEndPoint)user.ControlClient.Client.LocalEndPoint).Address.GetAddressBytes();
            byte[] port = BitConverter.GetBytes((short)((IPEndPoint)connection.DataListener.LocalEndpoint).Port);
            if(BitConverter.IsLittleEndian)
            {
                Array.Reverse(port);
            }
            user.SendResponse(FtpStatusCode.EnteringPassive, $"Entering Passive Mode ({address[0]},{address[1]},{address[2]},{address[3]},{port[0]},{port[1]})");
        }
    }
}