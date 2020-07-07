using System.IO;

namespace DouglasDwyer.ExtensibleFtp.Anonymous
{
    public class AnonymousFilesystem : IFtpFilesystem
    {
        public string RootDirectory { get; set; }

        public AnonymousFilesystem(string rootDirectory)
        {
            RootDirectory = rootDirectory;
        }

        public bool DirectoryExists(string path)
        {
            path = Path.Combine(RootDirectory, path);
            return Directory.Exists(path) && IsSubdirectoryOrRoot(path);
        }

        public bool FileExists(string path)
        {
            path = Path.Combine(RootDirectory, path);
            return File.Exists(path) && IsFileOfRoot(path);
        }

        public string[] GetFiles(string path)
        {
            if(!IsSubdirectoryOrRoot(path))
            {
                throw new FileNotFoundException();
            }
            return Directory.GetFiles(Path.Combine(RootDirectory, path));
        }

        public string[] GetSubdirectories(string path)
        {
            if (!IsSubdirectoryOrRoot(path))
            {
                throw new FileNotFoundException();
            }
            return Directory.GetDirectories(Path.Combine(RootDirectory, path));
        }

        public DirectoryInfo GetDirectoryInfo(string path)
        {
            if (!IsSubdirectoryOrRoot(path))
            {
                throw new FileNotFoundException();
            }
            return new DirectoryInfo(Path.Combine(RootDirectory, path));
        }

        public FileInfo GetFileInfo(string path)
        {
            if (!IsFileOfRoot(path))
            {
                throw new FileNotFoundException();
            }
            return new FileInfo(Path.Combine(RootDirectory, path));
        }

        public FileStream GetFileStream(string path)
        {
            if(!IsFileOfRoot(path))
            {
                throw new FileNotFoundException();
            }
            return File.OpenRead(Path.Combine(RootDirectory, path));
        }

        public string GetFilePermissions(string path)
        {
            return "-rwxrwxrwx";
        }

        public string GetDirectoryPermissions(string path)
        {
            return "drwxrwxrwx";
        }

        public void DeleteFile(string path)
        {
            if (!IsFileOfRoot(path))
            {
                throw new FileNotFoundException();
            }
            File.Delete(Path.Combine(RootDirectory, path));
        }

        public void DeleteDirectory(string path)
        {
            if (!IsSubdirectoryOfRoot(path))
            {
                throw new FileNotFoundException();
            }
            Directory.Delete(Path.Combine(RootDirectory, path), true);
        }

        public void CreateDirectory(string path)
        {
            if (!IsSubdirectoryOrRoot(Path.GetDirectoryName(path)))
            {
                throw new FileNotFoundException();
            }
            Directory.CreateDirectory(Path.Combine(RootDirectory, path));
        }

        public FileStream CreateFile(string path)
        {
            if(!IsFileOfRoot(path))
            {
                throw new FileNotFoundException();
            }
            return File.Create(Path.Combine(RootDirectory, path));
        }

        public void MoveDirectory(string oldPath, string newPath)
        {
            if(!IsSubdirectoryOfRoot(oldPath))
            {
                throw new FileNotFoundException();
            }
            else if(!IsSubdirectoryOrRoot(Path.GetDirectoryName(newPath)))
            {
                throw new FileNotFoundException();
            }
            Directory.Move(Path.Combine(RootDirectory, oldPath), Path.Combine(RootDirectory, newPath));
        }

        public void MoveFile(string oldPath, string newPath)
        {
            if (!IsFileOfRoot(oldPath))
            {
                throw new FileNotFoundException();
            }
            else if (!IsSubdirectoryOrRoot(Path.GetDirectoryName(newPath)))
            {
                throw new FileNotFoundException();
            }
            File.Move(Path.Combine(RootDirectory, oldPath), Path.Combine(RootDirectory, newPath));
        }

        private bool IsSubdirectoryOrRoot(string path)
        {
            DirectoryInfo di1 = new DirectoryInfo(RootDirectory);
            DirectoryInfo di2 = new DirectoryInfo(path);
            if(di1.FullName == di2.FullName)
            {
                return true;
            }
            while (di2.Parent != null)
            {
                if (di2.Parent.FullName == di1.FullName)
                {
                    return true;
                }
                else di2 = di2.Parent;
            }
            return false;
        }

        private bool IsSubdirectoryOfRoot(string path)
        {
            DirectoryInfo di1 = new DirectoryInfo(RootDirectory);
            DirectoryInfo di2 = new DirectoryInfo(path);
            while (di2.Parent != null)
            {
                if (di2.Parent.FullName == di1.FullName)
                {
                    return true;
                }
                else di2 = di2.Parent;
            }
            return false;
        }

        private bool IsFileOfRoot(string path)
        {
            return IsSubdirectoryOrRoot(Path.GetDirectoryName(path));
        }
    }
}