using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Utility.ADL;
using Utility.FastString;
using Utility.IO.Callbacks;
using Utility.IO.VirtualFS.Core;

namespace Utility.IO.VirtualFS
{
    /// <summary>
    /// Class that implements reading resources from an Assembly Manifest
    /// </summary>
    public static class ManifestReader
    {

        private static readonly ADLLogger<LogType> Logger =
            new ADLLogger<LogType>(ManifestIODebugConfig.Settings, "Manifest Reader");

        private static readonly Dictionary<string, AssemblyFile>
            _assemblyFiles = new Dictionary<string, AssemblyFile>();

        private static readonly List<Assembly> _loadedAssemblies = new List<Assembly>();
        private static readonly List<string> _unpackedFiles = new List<string>();

        public static bool IsRegistered(Assembly asm)
        {
            return _loadedAssemblies.Contains(asm);
        }

        /// <summary>
        /// Loads an Assembly List file
        /// </summary>
        /// <param name="filepath"></param>
        public static void LoadAssemblyListFromFile(string filepath)
        {
            if (IOManager.FileExists(filepath))
            {
                LoadAssemblyList(IOManager.GetStream(filepath));
            }
        }

        /// <summary>
        /// Logs all Files to the Console
        /// </summary>
        public static void ListAllFiles()
        {
            foreach (KeyValuePair<string, AssemblyFile> assemblyFile in _assemblyFiles)
            {
                Logger.Log(LogType.Log, assemblyFile.Key, 10);
            }
        }

        /// <summary>
        /// Loads the Assembly List from a stream
        /// </summary>
        /// <param name="data"></param>
        public static void LoadAssemblyList(Stream data)
        {
            TextReader tr = new StreamReader(data);

            string list = tr.ReadToEnd();
            tr.Close();
            string[] files = list.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < files.Length; i++)
            {
                Assembly asm = Assembly.Load(files[i].Replace("\r", "")); //Why windows. Why?
                if (asm != null)
                {
                    Logger.Log(LogType.Log, "Loading Assembly " + asm.GetName().Name, 10);
                    RegisterAssembly(asm);
                }
            }
        }

        /// <summary>
        /// Registers an Assembly into the IO Embedding Process
        /// </summary>
        /// <param name="asm"></param>
        public static void RegisterAssembly(Assembly asm)
        {
            if (_loadedAssemblies.Contains(asm))
            {
                return;
            }

            _loadedAssemblies.Add(asm);
            string[] files = asm.GetManifestResourceNames();
            Logger.Log(LogType.Log, "Adding Assembly: " + asm.GetName().Name, 10);
            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i].Remove(0, (asm.GetName().Name + ".").Length);
                if (_assemblyFiles.ContainsKey(file))
                {
                    Logger.Log(
                               LogType.Log,
                               "Overwriting File: " + file + " with version from assembly: " + asm.GetName().Name,
                               8
                              );
                    _assemblyFiles[file] = new AssemblyFile(false, files[i], asm);
                }
                else
                {
                    _assemblyFiles.Add(file, new AssemblyFile(false, files[i], asm));
                }
            }

            PrepareManifestFiles(asm);
        }

        private static AssemblyFile FileFactory(string file, bool compression, Assembly asm, AssetPointer ptr)
        {
            if (asm == null)
            {
                string[] files = new string[AssetPointer.GetPackageCount(ptr.Offset, ptr.Length, ptr.PackageSize)];
                string dir = Path.GetDirectoryName(UnSanitizeFilename(file));
                for (int i = 0; i < files.Length; i++)
                {
                    files[i] = $"{dir}/{ptr.PackageId + i}.pack";
                }

                return new IoPackedAssemblyFile(compression, files, ptr);
            }

            string[] f = new string[AssetPointer.GetPackageCount(ptr.Offset, ptr.Length, ptr.PackageSize)];
            string d = Path.GetDirectoryName(UnSanitizeFilename(file));
            for (int i = 0; i < f.Length; i++)
            {
                f[i] = SanitizeFilename($"{d}/{ptr.PackageId + i}.pack");
            }

            return new PackedAssemblyFile(compression, f, asm, ptr);
        }

        private static void PrepareAssemblyFiles(
            string packPrefix, string[] files, Assembly asm,
            AssemblyFileFactory factory)
        {
            int indexList = HasPackageFiles(files);
            if (indexList == -1)
            {
                return;
            }

            Logger.Log(LogType.Log, "Found Packed Files in Assembly: " + packPrefix, 10);

            string dir = packPrefix + "/packs";
            if (dir.StartsWith("/"))
            {
                dir = dir.Remove(0, 1);
            }

            string[] packs = IOManager.GetFiles(dir, "*.pack"); //Get only *.pack files
            Stream[] s = new Stream[packs.Length];
            for (int i = 0; i < packs.Length; i++)
            {
                s[i] = IOManager.GetStream(packs[i]);
            }

            Stream indexStream = IOManager.GetStream(files[indexList]);
            Dictionary<string, Tuple<int, MemoryStream>>
                filesToUnpack =
                    AssetPacker.UnpackAssets(
                                             indexStream,
                                             s
                                            ); //Get Files in the Packs that need to be unpacked to file system
            if (filesToUnpack.Count > 0)
            {
                UnpackAssets(filesToUnpack);
            }


            Stream idxStream = IOManager.GetStream(files[indexList]);
            List<Tuple<string, AssetPointer>> packedFiles =
                AssetPacker.GetPointers(idxStream, packs, out bool compression);
            Logger.Log(
                       LogType.Log,
                       $"Parsing {packedFiles.Count} File from {files[indexList]} in {packs.Length} Packages.",
                       10
                      );

            foreach (Tuple<string, AssetPointer> assetPointer in packedFiles)
            {
                string assemblyPath = SanitizeFilename(packPrefix + "/" + assetPointer.Item1);
                string virtualPath = SanitizeFilename(assetPointer.Item2.Path);
                if (_assemblyFiles.ContainsKey(virtualPath))
                {
                    Logger.Log(LogType.Log, $"Overwriting File: {assemblyPath} => {virtualPath}", 10);
                    _assemblyFiles[virtualPath] =
                        factory(
                                assemblyPath,
                                compression,
                                asm,
                                assetPointer.Item2
                               );
                }
                else
                {
                    _assemblyFiles.Add(
                                       virtualPath,
                                       factory(
                                               assemblyPath,
                                               compression,
                                               asm,
                                               assetPointer.Item2
                                              )
                                      );
                }
            }
        }


        private static void PrepareManifestFiles(Assembly loadedAssembly)
        {
            if (IOManager.DirectoryExists($"{loadedAssembly.GetName().Name}/packs"))
            {
                PrepareAssemblyFiles(
                                     loadedAssembly.GetName().Name,
                                     IOManager.GetFiles($"{loadedAssembly.GetName().Name}/packs", "*"),
                                     loadedAssembly,
                                     FileFactory
                                    );
            }
        }

        /// <summary>
        /// Prepares the Assembly Files for the loading process
        /// </summary>
        /// <param name="searchFileSystem">Flag to prefer the filesystem over the Assembly when preparing packaged files</param>
        public static void PrepareManifestFiles(bool searchFileSystem)
        {
            if (searchFileSystem)
            {
                if (IOManager.DirectoryExists("packs"))
                {
                    PrepareAssemblyFiles("", IOManager.GetFiles("packs", "*"), null, FileFactory);
                }
            }
            else
            {
                foreach (Assembly loadedAssembly in _loadedAssemblies)
                {
                    PrepareManifestFiles(loadedAssembly);
                }
            }
        }


        private static void UnpackAssets(Dictionary<string, Tuple<int, MemoryStream>> files)
        {
            Logger.Log(LogType.Log, $"Preparing to unpack {files.Count} Assets.. ", 10);
            foreach (KeyValuePair<string, Tuple<int, MemoryStream>> memoryStream in files)
            {
                bool hasUnpackedVersion = _unpackedFiles.Contains(memoryStream.Key);
                if (hasUnpackedVersion)
                {
                    File.Delete(memoryStream.Key);
                }

                bool shouldWrite = hasUnpackedVersion || !File.Exists(memoryStream.Key);
                if (shouldWrite)
                {
                    byte[] buf = new byte[memoryStream.Value.Item1];
                    memoryStream.Value.Item2.Position = 0;
                    memoryStream.Value.Item2.Read(buf, 0, buf.Length);

                    List<string> folders = new List<string>();
                    string curFolder = Path.GetDirectoryName(memoryStream.Key);
                    folders.Add(curFolder);
                    while (curFolder.Trim() != "\\")
                    {
                        if (string.IsNullOrEmpty(curFolder))
                        {
                            break;
                        }

                        folders.Add(curFolder);
                        curFolder = Path.GetDirectoryName(curFolder);
                    }

                    for (int i = 0; i < folders.Count; i++)
                    {
                        if (!Directory.Exists(folders[i]))
                        {
                            Logger.Log(LogType.Log, $"Creating folder {folders[i]} on disk.", 8);
                            Directory.CreateDirectory(".\\" + folders[i]);
                        }
                    }

                    if (!hasUnpackedVersion)
                    {
                        _unpackedFiles.Add(memoryStream.Key);
                    }

                    File.WriteAllBytes(memoryStream.Key, buf);
                }
            }
        }

        private static int HasPackageFiles(string[] files)
        {
            int id = -1;
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].EndsWith("index.xml"))
                {
                    id = i;
                }
            }

            return id;
        }

        /// <summary>
        /// Returns the Manifest Stream by path
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static Stream GetStreamByPath(string filepath)
        {
            string path = SanitizeFilename(filepath);

            if (!_assemblyFiles.ContainsKey(path))
            {
                throw new ManifestReaderException("Could not load file: " + filepath);
            }

            return _assemblyFiles[path].GetFileStream();
        }

        /// <summary>
        /// Returns true if this directory is existing(has minor trouble with . characters in filenames)
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool DirectoryExists(string path)
        {
            string p = SanitizeFilename(path);
            foreach (KeyValuePair<string, AssemblyFile> assemblyFile in _assemblyFiles)
            {
                if (assemblyFile.Key.StartsWith(p))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns a list of files that are in the specified path and satisfy the search pattern
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchPattern">* = all files</param>
        /// <returns></returns>
        public static string[] GetFiles(string path, string searchPattern)
        {
            string[] files = _assemblyFiles.Keys.ToArray();
            string p = SanitizeFilename(path);
            List<string> ret = new List<string>();
            string[] searchParts = searchPattern.Split('*');
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].StartsWith(p) /* && (files[i].EndsWith(searchPattern) || searchPattern == "*")*/)
                {
                    string filePart = files[i].Replace(p, "");
                    int lastIdx = 0;
                    bool valid = true;
                    for (int j = 0; j < searchParts.Length; j++)
                    {
                        if (searchParts.Length == j + 1 && !string.IsNullOrEmpty(searchParts[j]))
                        {
                            valid = filePart.EndsWith(searchParts[j]);
                            break;
                        }

                        if (string.IsNullOrEmpty(searchParts[j]))
                        {
                            continue;
                        }

                        int idx = FString.FastIndexOf(ref filePart, searchParts[j], lastIdx);
                        if (lastIdx > idx)
                        {
                            valid = false;
                            break;
                        }
                    }

                    if (valid)
                    {
                        ret.Add(UnSanitizeFilename(files[i]));
                    }
                }
            }

            return ret.ToArray();
        }


        /// <summary>
        /// Turns a Filepath that refers to an assembly file into a filepath
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static string UnSanitizeFilename(string filepath)
        {
            string ret = filepath.Replace(".", "/");
            int idx = ret.LastIndexOf("/");
            ret = ret.Remove(idx, 1).Insert(idx, ".");


            return ret;
        }

        /// <summary>
        /// Turns a Filepath that refers to an file on the filesystem into a filepath for assembly files
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static string SanitizeFilename(string filepath)
        {
            if (filepath[0] == '/' || filepath[0] == '\\')
            {
                filepath = filepath.Remove(0, 1);
            }

            if (filepath[filepath.Length - 1] == '/' || filepath[filepath.Length - 1] == '\\')
            {
                filepath = filepath.Remove(filepath.Length - 1, 1);
            }

            string clean = PathTools.CleanPath(filepath);

            return clean.Replace("/", ".").Replace("\\", ".");
        }

        /// <summary>
        /// Returns true if this file exists
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static bool Exists(string filepath)
        {
            string clean = PathTools.CleanPath(filepath);
            string p = SanitizeFilename(clean);
            return _assemblyFiles.ContainsKey(p);
        }

        /// <summary>
        /// Clears all unpacked files from the filesystem
        /// </summary>
        public static void ClearUnpackedFiles()
        {
            for (int i = 0; i < _unpackedFiles.Count; i++)
            {
                Logger.Log(LogType.Log, "Removing File from Filesystem: " + _unpackedFiles[i], 8);
                File.Delete(_unpackedFiles[i]);
            }
        }

        private delegate AssemblyFile
            AssemblyFileFactory(string file, bool compression, Assembly asm, AssetPointer ptr);

    }
}