using System.IO;

namespace Utility.IO.Callbacks
{
    public class DefaultIOCallback : IOCallback
    {

        public virtual bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public virtual bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public virtual string ReadText(string path)
        {
            return File.ReadAllText(path);
        }

        public virtual string[] ReadLines(string path)
        {
            return File.ReadAllLines(path);
        }

        public virtual Stream GetStream(string path)
        {
            return File.OpenRead(path);
        }

        public virtual string[] GetFiles(string path, string searchPattern)
        {
            return Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories);
        }

        public virtual string[] GetFiles(string path)
        {
            return GetFiles(path, "*.*");
        }

    }
}