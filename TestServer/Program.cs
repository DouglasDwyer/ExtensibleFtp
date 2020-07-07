using System;
using System.Collections.Generic;
using System.Net.Sockets;
using DouglasDwyer.ExtensibleFtp;
using DouglasDwyer.ExtensibleFtp.Anonymous;

namespace TestServer
{
    class Program
    {
        public static ExtensibleFtpServer Server = new ExtensibleFtpServer(new AnonymousAuthenticator("D:/"));

        static void Main(string[] args)
        {
            Server.StartAsync(21);
            Console.ReadKey();
            Server.Stop();
        }
    }
}
