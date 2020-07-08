using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DouglasDwyer.ExtensibleFtp
{
    /// <summary>
    /// Represents an FTP error which occured during processing a user request.  When an FTP exception is thrown, its information is relayed to the FTP client.
    /// </summary>
    public class FtpException : Exception
    {
        /// <summary>
        /// The status code associated with this error.
        /// </summary>
        public int StatusCode { get; protected set; }

        /// <summary>
        /// Creates a new instance of the <see cref="FtpException"/> class.
        /// </summary>
        public FtpException() { }
        /// <summary>
        /// Creates a new instance of the <see cref="FtpException"/> class with the specified data.
        /// </summary>
        /// <param name="code">The FTP status code associated with this error.</param>
        /// <param name="error">A message describing the error itself.</param>
        public FtpException(int code, string error) : base(error)
        {
            StatusCode = code;
        }
        /// <summary>
        /// Creates a new instance of the <see cref="FtpException"/> class with the specified data.
        /// </summary>
        /// <param name="code">The FTP status code associated with this error.</param>
        /// <param name="error">A message describing the error itself.</param>
        public FtpException(FtpStatusCode code, string error) : base(error)
        {
            StatusCode = (int)code;
        }
    }
}
