namespace DouglasDwyer.ExtensibleFtp
{
    /// <summary>
    /// Used to describe the way that files should be transferred over FTP.
    /// </summary>
    public enum TransferMode
    {
        /// <summary>
        /// Files should be transferred using ASCII text encoding.
        /// </summary>
        ASCII,
        /// <summary>
        /// Files should be transferred using their raw binary data.
        /// </summary>
        Binary,
        /// <summary>
        /// Files should be transferred using EBCDIC text encoding.
        /// </summary>
        EBCDIC
    }
}