using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using DouglasDwyer.ExtensibleFtp;

namespace TestServer
{
    public class BasicFilesystem : IFtpFilesystem
    {
        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public DirectoryInfo GetDirectoryInfo(string path)
        {
            return new DirectoryInfo(path);
        }

        public FileInfo GetFileInfo(string path)
        {
            return new FileInfo(path);
        }

        public string GetFilePermissions(string path)
        {
            return "-rwxrwxrwx";
        }

        public string GetDirectoryPermissions(string path)
        {
            return "drwxrwxrwx";
        }

        public string[] GetFiles(string path)
        {
            return Directory.GetFiles(path);
        }

        public FileStream GetFileStream(string path)
        {
            return File.OpenRead(path);
        }

        public string[] GetSubdirectories(string path)
        {
            return Directory.GetDirectories(path);
        }

        public void DeleteFile(string path)
        {
            File.Delete(path);
        }

        public void DeleteDirectory(string path)
        {
            Directory.Delete(path, true);
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public FileStream CreateFile(string path)
        {
            return File.Create(path);
        }

        public void MoveDirectory(string oldPath, string newPath)
        {
            Directory.Move(oldPath, newPath);
        }

        public void MoveFile(string oldPath, string newPath)
        {
            File.Move(oldPath, newPath);
        }
    }
}
