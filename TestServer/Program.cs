using System;
using System.Collections.Generic;
using System.Net.Sockets;
using DouglasDwyer.ExtensibleFtp;
using DouglasDwyer.ExtensibleFtp.Anonymous;

namespace TestServer
{
    class Program
    {
        public static ExtensibleFtpServer Server = new ExtensibleFtpServer(new AnonymousAuthenticator("D:/Cross The Road Electronics/"));

        static void Main(string[] args)
        {
            Server.Start(21);
            Console.ReadKey();
            Server.Stop();
        }
    }
}
