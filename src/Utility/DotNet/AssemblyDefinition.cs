using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Utility.DotNet
{
    [Serializable]
    public class AssemblyDefinition
    {

        public AssemblyDefinition()
        {
        }

        public AssemblyDefinition(
            string assemblyName, string buildConfiguration = "Release",
            bool noTargetRuntime = false, string buildTargetRuntime = "win-x64")
        {
            AssemblyName = assemblyName;
            BuildConfiguration = buildConfiguration;
            NoTargetRuntime = noTargetRuntime;
            BuildTargetRuntime = buildTargetRuntime;
        }

        public List<ModuleDefinition> IncludedModules { get; set; } = new List<ModuleDefinition>();

        public bool NoTargetRuntime { get; set; }

        public string AssemblyName { get; set; } = "TestAssembly";

        public string BuildConfiguration { get; set; } = "Release";

        public string BuildTargetRuntime { get; set; } = "win-x64";

        public static AssemblyDefinition Load(string path)
        {
            XmlSerializer xs = new XmlSerializer(typeof(AssemblyDefinition));
            Stream s = File.OpenRead(path);
            AssemblyDefinition ret = (AssemblyDefinition) xs.Deserialize(s);
            s.Close();
            return ret;
        }

        public static void Save(string path, AssemblyDefinition definition)
        {
            XmlSerializer xs = new XmlSerializer(typeof(AssemblyDefinition));
            Stream s = File.Create(path);
            xs.Serialize(s, definition);
            s.Close();
        }

    }
}