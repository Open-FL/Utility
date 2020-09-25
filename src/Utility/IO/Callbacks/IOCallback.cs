using System.IO;

namespace Utility.IO.Callbacks
{
    public interface IOCallback
    {

        string ReadText(string path);

        string[] ReadLines(string path);

        Stream GetStream(string path);

        bool FileExists(string file);

        bool DirectoryExists(string file);

        string[] GetFiles(string path, string searchPattern = "*.*");

    }
}