using Plugin.NetStandardStorage.Abstractions.Interfaces;
using Plugin.NetStandardStorage.Abstractions.Types;
using System;
using System.Collections.Generic;
using System.IO;
using TUtils.Error;

namespace TUtils.IO
{
    /// <summary>
    /// A file implementation for windows that supports plugin.netstandardstorage's IFile abstraction.
    /// Only used for the unit tests.
    /// </summary>
    public class WindowsFile : IFile
    {
        public string Name { get; private set; }

        public string FullPath { get; private set; }

        public WindowsFile(string path)
        {
            if (!File.Exists(path))
            {
                using (File.Create(path)) { }
            }

            this.FullPath = path;
            this.Name = Path.GetFileName(path);
        }

        public void Delete()
        {
            File.Delete(this.FullPath);
            this.FullPath = string.Empty;
            this.Name = string.Empty;
        }

        public void Move(string newPath, NameCollisionOption option = NameCollisionOption.ReplaceExisting)
        {
            switch (option)
            {
                case NameCollisionOption.GenerateUniqueName:
                    newPath = Path.GetDirectoryName(newPath) + Path.DirectorySeparatorChar
                        + Guid.NewGuid() + ".windowsfile";
                    File.Move(this.FullPath, newPath);
                    break;

                case NameCollisionOption.ReplaceExisting:
                    File.Move(this.FullPath, newPath);
                    break;

                case NameCollisionOption.FailIfExists:
                    File.Move(this.FullPath, newPath);
                    break;
            }

            this.FullPath = newPath;
        }

        public Stream Open(FileAccess fileAccess)
        {
            switch (fileAccess)
            {
                case FileAccess.Read:
                    return File.OpenRead(this.FullPath);

                case FileAccess.Write:
                    return File.OpenWrite(this.FullPath);

                case FileAccess.ReadWrite:
                    return File.Open(this.FullPath, FileMode.OpenOrCreate);

                default:
                    throw new UnexpectedMemberException("An unexpected member of the FileAccess enum was encounter");
            }
        }

        public void Rename(string newName, NameCollisionOption option = NameCollisionOption.FailIfExists)
        {
            string newFilePath = Directory.GetDirectoryRoot(this.FullPath) + newName;
            this.Move(newFilePath);
        }

        public void WriteAllBytes(byte[] bytes)
        {
            File.WriteAllBytes(this.FullPath, bytes);
        }

        public void WriteAllLines(IEnumerable<string> contents)
        {
            File.WriteAllLines(this.FullPath, contents);
        }

        public void WriteAllText(string contents)
        {
            File.WriteAllText(this.FullPath, contents);
        }
    }
}