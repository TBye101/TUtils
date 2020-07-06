using Plugin.NetStandardStorage.Abstractions.Interfaces;
using Plugin.NetStandardStorage.Abstractions.Types;
using System.Collections.Generic;
using System.IO;
using TUtils.Collections.Enumeration;

namespace TUtils.IO
{
    /// <summary>
    /// A folder implementation for windows that supports plugin.netstandardstorage's IFolder abstraction.
    /// Only used for the unit test assembly.
    /// </summary>
    public class WindowsFolder : IFolder
    {
        public static IFolder ExecutingAssemblyDirectory = new WindowsFolder(Directory.GetCurrentDirectory());

        public string Name { get; private set; }

        public string FullPath { get; private set; }

        public WindowsFolder(string path)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException();

            this.FullPath = path;
            this.Name = Path.GetDirectoryName(path);
        }

        public bool CheckFileExists(string name)
        {
            IEnumerable<string> fileNames = Directory.EnumerateFiles(this.FullPath);
            foreach (string item in fileNames)
            {
                if (item.Equals(name))
                    return true;
            }

            return false;
        }

        public bool CheckFolderExists(string name)
        {
            return Directory.Exists(this.FullPath + Path.DirectorySeparatorChar + name);
        }

        public IFile CreateFile(string name, CreationCollisionOption option)
        {
            return new WindowsFile(this.FullPath + Path.DirectorySeparatorChar + name);
        }

        public IFolder CreateFolder(string name, CreationCollisionOption option)
        {
            DirectoryInfo dirInfo =
                Directory.CreateDirectory(this.FullPath + Path.DirectorySeparatorChar + name);
            return new WindowsFolder(dirInfo.FullName);
        }

        public void Delete()
        {
            Directory.Delete(this.FullPath);
            this.FullPath = string.Empty;
            this.Name = string.Empty;
        }

        public void DeleteRecursively()
        {
            this.Delete();//Windows delete recursively... right?
        }

        public IFile GetFile(string name)
        {
            string filepath = this.FullPath + Path.DirectorySeparatorChar + name;
            if (!File.Exists(filepath))
                throw new FileNotFoundException(name);

            return new WindowsFile(filepath);
        }

        public IList<IFile> GetFiles()
        {
            List<IFile> fetchedFiles = new List<IFile>();
            string[] files = Directory.GetFiles(this.FullPath);
            files.Foreach<string>((file) =>
            {
                fetchedFiles.Add(new WindowsFile(file));
            });

            return fetchedFiles;
        }

        public IFolder GetFolder(string name)
        {
            if (!this.CheckFolderExists(name))
                throw new DirectoryNotFoundException();

            return new WindowsFolder(this.FullPath + Path.DirectorySeparatorChar + name);
        }

        public IList<IFolder> GetFolders()
        {
            List<IFolder> folders = new List<IFolder>();
            IEnumerable<string> directories = Directory.GetDirectories(this.FullPath);
            foreach (string item in directories)
            {
                folders.Add(new WindowsFolder(item));
            }
            return folders;
        }
    }
}