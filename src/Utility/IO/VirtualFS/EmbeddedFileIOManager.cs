using System.Collections.Generic;
using System.IO;
using System.Linq;

using Utility.ADL;
using Utility.IO.Callbacks;

namespace Utility.IO.VirtualFS
{
    /// <summary>
    /// Wrapper for Specific System.IO Calls
    /// It will resolve the filename either with files from the disk or with files embedded in an assembly
    /// </summary>
    public class EmbeddedFileIOManager : IOCallback
    {

        private static readonly ADLLogger<LogType> Logger =
            new ADLLogger<LogType>(ManifestIODebugConfig.Settings, "File IOManager");

        /// <summary>
        /// Returns true if the file exists on either the disk or in the assembly
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool FileExists(string filename)
        {
            bool isFile = File.Exists(filename);
            bool isManifest = ManifestReader.Exists(filename);
            return isFile || isManifest;
        }

        /// <summary>
        /// Returns true if the folder exists on either the disk or in the assembly
        /// </summary>
        /// <param name="foldername"></param>
        /// <returns></returns>
        public bool DirectoryExists(string foldername)
        {
            bool isFile = Directory.Exists(foldername);
            bool isManifest = ManifestReader.DirectoryExists(foldername);
            return isFile || isManifest;
        }

        /// <summary>
        /// Reads all lines from the file provided
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string[] ReadLines(string path)
        {
            return ReadText(path).Split('\n');
        }

        /// <summary>
        /// Reads all Text from a file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string ReadText(string path)
        {
            TextReader tr = new StreamReader(GetStream(path));
            string ret = tr.ReadToEnd();
            tr.Close();
            return ret;
        }

        /// <summary>
        /// Returns files in a specfied directory
        /// </summary>
        /// <param name="foldername"></param>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        public string[] GetFiles(string foldername, string searchPattern)
        {
            bool folderExists = DirectoryExists(foldername);

            if (!folderExists)
            {
                throw new InvalidFilePathException(foldername);
            }

            List<string> files = new List<string>();
            if (Directory.Exists(foldername))
            {
                Logger.Log(LogType.Log, foldername + " Found in File System.", 5);
                files = Directory.GetFiles(foldername, searchPattern).ToList();
            }

            if (ManifestReader.DirectoryExists(foldername))
            {
                Logger.Log(LogType.Log, foldername + " Found in Assembly Manifest.", 5);
                files.AddRange(ManifestReader.GetFiles(foldername, searchPattern /*.Replace("*", "")*/));
            }

            return files.ToArray();
        }

        /// <summary>
        /// Returns the byte stream of the file specified
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public Stream GetStream(string filename)
        {
            if (File.Exists(filename))
            {
                Logger.Log(LogType.Log, filename + " Found in File System.", 5);
                return File.OpenRead(filename);
            }

            if (ManifestReader.Exists(filename))
            {
                Logger.Log(LogType.Log, filename + " Found in Assembly Manifest.", 5);
                return ManifestReader.GetStreamByPath(filename);
            }

            throw new InvalidFilePathException(filename);
        }

        public static void Initialize()
        {
            if (!(IOManager.Callback is EmbeddedFileIOManager))
            {
                IOManager.SetIOCallback(new EmbeddedFileIOManager());
            }
        }

    }
}