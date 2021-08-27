using System.IO;

namespace DouglasDwyer.ExtensibleFtp.Anonymous
{
    /// <summary>
    /// Represents a filesystem where users have full control over any files under a certain root directory.
    /// </summary>
    public class AnonymousFilesystem : IFtpFilesystem
    {
        /// <summary>
        /// The root directory in which users should be allowed to edit files.
        /// </summary>
        public string RootDirectory { get; set; }

        /// <summary>
        /// Creates a new <see cref="AnonymousFilesystem"/> instance with the specified data.
        /// </summary>
        /// <param name="rootDirectory">The root directory in which users should be allowed to edit files.</param>
        public AnonymousFilesystem(string rootDirectory)
        {
            RootDirectory = rootDirectory;
        }

        /// <summary>
        /// Checks whether a directory exists.
        /// </summary>
        /// <param name="path">The directory to check.</param>
        /// <returns>Whether the directory exists.</returns>
        public bool DirectoryExists(string path)
        {
            path = CombinePaths(RootDirectory, path);
            return Directory.Exists(path) && IsSubdirectoryOrRoot(path);
        }
        /// <summary>
        /// Checks whether a file exists.
        /// </summary>
        /// <param name="path">The file to check.</param>
        /// <returns>Whether the file exists.</returns>
        public bool FileExists(string path)
        {
            path = CombinePaths(RootDirectory, path);
            return File.Exists(path) && IsFileOfRoot(path);
        }
        /// <summary>
        /// Retrieves all the files in the specified directory.
        /// </summary>
        /// <param name="path">The path of the directory.</param>
        /// <returns>A list of files.</returns>
        public string[] GetFiles(string path)
        {
            path = CombinePaths(RootDirectory, path);
            if (!IsSubdirectoryOrRoot(path))
            {
                throw new FileNotFoundException();
            }
            string rootFullName = new DirectoryInfo(RootDirectory).FullName;
            string[] files = Directory.GetFiles(path);
            for(int i = 0; i < files.Length; i++)
            {
                files[i] = files[i].Substring(rootFullName.Length);
            }
            return files;
        }
        /// <summary>
        /// Retrieves all the subdirectories in the specified directory.
        /// </summary>
        /// <param name="path">The path of the directory.</param>
        /// <returns>A list of directories.</returns>
        public string[] GetSubdirectories(string path)
        {
            path = CombinePaths(RootDirectory, path);
            if (!IsSubdirectoryOrRoot(path))
            {
                throw new FileNotFoundException();
            }
            string rootFullName = new DirectoryInfo(RootDirectory).FullName;
            string[] directories = Directory.GetDirectories(path);
            for (int i = 0; i < directories.Length; i++)
            {
                directories[i] = directories[i].Substring(rootFullName.Length);
            }
            return directories;
        }
        /// <summary>
        /// Retrieves a <see cref="DirectoryInfo"/> object that contains information about a specified directory.
        /// </summary>
        /// <param name="path">The path of the directory.</param>
        /// <returns>An object containing information about the directory.</returns>
        public DirectoryInfo GetDirectoryInfo(string path)
        {
            path = CombinePaths(RootDirectory, path);
            if (!IsSubdirectoryOrRoot(path))
            {
                throw new FileNotFoundException();
            }
            return new DirectoryInfo(path);
        }
        /// <summary>
        /// Retrieves a <see cref="FileInfo"/> object that contains information about a specified file.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <returns>An object containing information about the file.</returns>
        public FileInfo GetFileInfo(string path)
        {
            path = CombinePaths(RootDirectory, path);
            if (!IsFileOfRoot(path))
            {
                throw new FileNotFoundException();
            }
            return new FileInfo(path);
        }
        /// <summary>
        /// Retrieves a read-only file stream for the specified file.
        /// </summary>
        /// <param name="path">The path of the file to read.</param>
        /// <returns>A read-only file stream.</returns>
        public FileStream GetFileStream(string path)
        {
            path = CombinePaths(RootDirectory, path);
            if (!IsFileOfRoot(path))
            {
                throw new FileNotFoundException();
            }
            return File.OpenRead(path);
        }
        /// <summary>
        /// Gets the permission listing for the specified file.
        /// </summary>
        /// <param name="path">The path of the file to get information about.</param>
        /// <param name="identity">The identity of the current user.</param>
        /// <returns>A unix-formatted permission string.</returns>
        public string GetFilePermissions(string path, IFtpIdentity identity)
        {
            return "-rwxrwxrwx";
        }
        /// <summary>
        /// Gets the permission listing for the specified directory.
        /// </summary>
        /// <param name="path">The path of the directory to get information about.</param>
        /// <param name="identity">The identity of the current user.</param>
        /// <returns>A unix-formatted permission string.</returns>
        public string GetDirectoryPermissions(string path, IFtpIdentity identity)
        {
            return "drwxrwxrwx";
        }
        /// <summary>
        /// Deletes a file.
        /// </summary>
        /// <param name="path">The path of the file to delete.</param>
        public void DeleteFile(string path)
        {
            path = CombinePaths(RootDirectory, path);
            if (!IsFileOfRoot(path))
            {
                throw new FileNotFoundException();
            }
            File.Delete(path);
        }
        /// <summary>
        /// Deletes a directory recursively, removing all files and subdirectories.
        /// </summary>
        /// <param name="path">The path of the directory to delete.</param>
        public void DeleteDirectory(string path)
        {
            path = CombinePaths(RootDirectory, path);
            if (!IsSubdirectoryOfRoot(path))
            {
                throw new FileNotFoundException();
            }
            Directory.Delete(path, true);
        }
        /// <summary>
        /// Creates a new directory.
        /// </summary>
        /// <param name="path">The name of the directory to create.</param>
        public void CreateDirectory(string path)
        {
            path = CombinePaths(RootDirectory, path);
            if (!IsSubdirectoryOrRoot(Path.GetDirectoryName(path)))
            {
                throw new FileNotFoundException();
            }
            Directory.CreateDirectory(path);
        }
        /// <summary>
        /// Creates a new file.
        /// </summary>
        /// <param name="path">The path of the file to create.</param>
        /// <returns>A file stream for writing to the new file.</returns>
        public FileStream CreateFile(string path)
        {
            path = CombinePaths(RootDirectory, path);
            if (!IsFileOfRoot(path))
            {
                throw new FileNotFoundException();
            }
            return File.Create(path);
        }
        /// <summary>
        /// Moves a directory, changing its path.
        /// </summary>
        /// <param name="oldPath">The current path of the directory.</param>
        /// <param name="newPath">The new path of the directory.</param>
        public void MoveDirectory(string oldPath, string newPath)
        {
            oldPath = CombinePaths(RootDirectory, oldPath);
            newPath = CombinePaths(RootDirectory, newPath);
            if (!IsSubdirectoryOfRoot(oldPath))
            {
                throw new FileNotFoundException();
            }
            else if(!IsSubdirectoryOrRoot(Path.GetDirectoryName(newPath)))
            {
                throw new FileNotFoundException();
            }
            Directory.Move(oldPath, newPath);
        }
        /// <summary>
        /// Moves a file, changing its path.
        /// </summary>
        /// <param name="oldPath">The current path of the file.</param>
        /// <param name="newPath">The new path of the file.</param>
        public void MoveFile(string oldPath, string newPath)
        {
            oldPath = CombinePaths(RootDirectory, oldPath);
            newPath = CombinePaths(RootDirectory, newPath);
            if (!IsFileOfRoot(oldPath))
            {
                throw new FileNotFoundException();
            }
            else if (!IsSubdirectoryOrRoot(Path.GetDirectoryName(newPath)))
            {
                throw new FileNotFoundException();
            }
            File.Move(oldPath, newPath);
        }

        private bool IsSubdirectoryOrRoot(string path)
        {
            DirectoryInfo di1 = new DirectoryInfo(RootDirectory);
            DirectoryInfo di2 = new DirectoryInfo(path);
            if(di1.FullName.TrimEnd('\\') == di2.FullName.TrimEnd('\\'))
            {
                return true;
            }
            while (di2.Parent != null)
            {
                if (di2.Parent.FullName.TrimEnd('\\') == di1.FullName.TrimEnd('\\'))
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
                if (di2.Parent.FullName.TrimEnd('\\') == di1.FullName.TrimEnd('\\'))
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

        private string CombinePaths(params string[] args)
        {
            string output = "";
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].EndsWith("/"))
                {
                    output += args[i];
                }
                else
                {
                    if(i == args.Length - 1)
                    {
                        output += args[i];
                    }
                    else
                    {
                        output += args[i] + "/";
                    }
                }
            }
            return output;
        }
    }
}