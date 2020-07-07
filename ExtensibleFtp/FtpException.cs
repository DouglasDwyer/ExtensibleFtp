using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DouglasDwyer.ExtensibleFtp
{
    public class FtpException : Exception
    {
        public int HttpCode { get; protected set; }

        public FtpException() { }
        public FtpException(int code, string error) : base(error)
        {
            HttpCode = code;
        }

        public FtpException(FtpStatusCode code, string error) : base(error)
        {
            HttpCode = (int)code;
        }
    }
}
