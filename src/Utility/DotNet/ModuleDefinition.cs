using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using Utility.DotNet.ProjectParsing;

namespace Utility.DotNet
{
    [Serializable]
    public class ModuleDefinition
    {

        public ModuleDefinition()
        {
        }

        public ModuleDefinition(
            string name, string rootDirectory, CSharpReference[] packages,
            CSharpReference[] projects, CSharpReference[] embeddedFiles)
        {
            Packages = packages;
            Projects = projects;
            EmbeddedFiles = embeddedFiles;
            Name = name;
            RootDirectory = rootDirectory;
        }

        public string Name { get; set; }

        public string RootDirectory { get; set; }

        public string[] ScriptFiles =>
            Directory.GetFiles(RootDirectory, "*.cs", SearchOption.AllDirectories)
                     .Where(x => !x.Contains("\\bin\\") && !x.Contains("\\obj\\")).ToArray();

        public CSharpReference[] Packages { get; set; }

        public CSharpReference[] Projects { get; set; }

        public CSharpReference[] EmbeddedFiles { get; set; }

        public static ModuleDefinition Load(string path)
        {
            XmlSerializer xs = new XmlSerializer(typeof(ModuleDefinition));
            Stream s = File.OpenRead(path);
            ModuleDefinition ret = (ModuleDefinition) xs.Deserialize(s);
            s.Close();
            return ret;
        }


        public static void Save(string path, ModuleDefinition definition)
        {
            XmlSerializer xs = new XmlSerializer(typeof(ModuleDefinition));
            Stream s = File.Create(path);
            xs.Serialize(s, definition);
            s.Close();
        }

    }
}