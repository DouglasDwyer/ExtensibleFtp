using System;
using System.IO;

namespace DouglasDwyer.ExtensibleFtp
{
    public interface IFtpFilesystem
    {
        bool DirectoryExists(string path);
        bool FileExists(string path);
        string[] GetFiles(string path);
        string[] GetSubdirectories(string path);
        DirectoryInfo GetDirectoryInfo(string path);
        FileInfo GetFileInfo(string path);
        FileStream GetFileStream(string path);
        string GetFilePermissions(string path);
        string GetDirectoryPermissions(string path);
        void DeleteFile(string path);
        void DeleteDirectory(string path);
        void CreateDirectory(string path);
        FileStream CreateFile(string path);
        void MoveDirectory(string oldPath, string newPath);
        void MoveFile(string oldPath, string newPath);
    }
}
