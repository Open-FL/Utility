using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Utility.ADL;
using Utility.FastString;

namespace Utility.CommandRunner.BuiltInCommands.SetSettings
{
    public class SetSettingsCommand : AbstractCommand
    {

        private readonly Dictionary<string, FieldInformations> RootNodes;


        public SetSettingsCommand(Dictionary<string, FieldInformations> rootNodes, string[] keys = null) :
            base(keys ?? new[] { "--set-settings", "-ss" }, "Sets the Settings")
        {
            CommandAction = (info, strings) => RunReflection(info);
            RootNodes = rootNodes;
            Logger.Log(LogType.Log, "Root Nodes: " + RootNodes.Count, 1);
        }

        public List<string> AllPaths
        {
            get
            {
                List<string> ret = new List<string>();
                RootNodes.Select(x => x.Value.Fields.Select(y => y.Key)).ToList().ForEach(x => ret.AddRange(x));
                return ret;
            }
        }

        private void RunReflection(StartupArgumentInfo info)
        {
            int maxI = info.GetCommandEntries(CommandKeys[0]);
            for (int i = 0; i < maxI; i++)
            {
                ReflectData(info.GetValues(CommandKeys[0], i).ToArray());
            }
        }

        private void ReflectData(string[] args)
        {
            bool hasErrors = false;
            for (int i = 0; i < args.Length; i++)
            {
                string[] parts = args[i].Split(':');
                string fullpath = parts[0];
                Logger.Log(LogType.Log, "Trying to Find Field: " + fullpath, 3);
                if (parts.Length < 2 || fullpath.IndexOf('.') == -1)
                {
                    continue;
                }

                if (parts.Length > 2)
                {
                    parts[1] = parts.Reverse().Take(parts.Length - 2).Reverse().Unpack(":");
                }

                string root = fullpath.Substring(0, fullpath.IndexOf('.'));
                string data = parts[1];
                if (RootNodes.ContainsKey(root))
                {
                    FieldInformations fis = RootNodes[root];
                    if (fis.Fields.ContainsKey(fullpath))
                    {
                        Logger.Log(LogType.Log, "Setting " + fullpath + " to " + data, 1);
                        FieldInformation fi = fis.Fields[fullpath];
                        if (fi.info.FieldType == typeof(float))
                        {
                            fi.info.SetValue(fi.reference, float.Parse(data));
                        }
                        else if (fi.info.FieldType == typeof(int))
                        {
                            fi.info.SetValue(fi.reference, int.Parse(data));
                        }
                        else if (fi.info.FieldType == typeof(string))
                        {
                            fi.info.SetValue(fi.reference, data);
                        }
                        else if (fi.info.FieldType == typeof(bool))
                        {
                            fi.info.SetValue(fi.reference, bool.Parse(data));
                        }
                    }
                    else
                    {
                        hasErrors = true;
                        Logger.Log(LogType.Error, "Can not Find a Field with Path: " + fullpath, 1);
                    }
                }
                else
                {
                    hasErrors = true;
                    Logger.Log(LogType.Error, "Can not Find a Field with Path: " + fullpath, 1);
                }
            }

            if (hasErrors)
            {
                Logger.Log(LogType.Error, "Command completed with Errors.", 1);
                Logger.Log(LogType.Log, "Available Options: \n" + AllPaths, 1);
            }
        }

        private static void RecursiveAddFields(
            string parentPath, object parentRef, FieldInfo current,
            Dictionary<string, FieldInformation> infos)
        {
            string thisPath = parentPath + "." + current.Name;
            if (current.FieldType == typeof(float) ||
                current.FieldType == typeof(string) ||
                current.FieldType == typeof(int) ||
                current.FieldType == typeof(bool))
            {
                infos.Add(
                          thisPath,
                          new FieldInformation
                          {
                              reference = parentRef,
                              info = current,
                              path = thisPath
                          }
                         );
            }
            else
            {
                object newref = current.GetValue(parentRef);
                FieldInfo[] subs = GetFieldsOfType(newref.GetType());
                foreach (FieldInfo fieldInfo in subs)
                {
                    RecursiveAddFields(thisPath, newref, fieldInfo, infos);
                }
            }
        }

        private static FieldInfo[] GetFieldsOfType(Type t)
        {
            return t.GetFields();
        }


        private static Dictionary<string, FieldInformation> GenerateFieldInformation(string rootNode, object o)
        {
            Dictionary<string, FieldInformation> ret = new Dictionary<string, FieldInformation>();
            FieldInfo[] fis = GetFieldsOfType(o.GetType());
            foreach (FieldInfo fieldInfo in fis)
            {
                RecursiveAddFields(rootNode, o, fieldInfo, ret);
            }

            return ret;
        }


        public static Dictionary<string, FieldInformations> Create(params ObjectFieldContainer[] objs)
        {
            Dictionary<string, FieldInformations> ret = new Dictionary<string, FieldInformations>();
            for (int i = 0; i < objs.Length; i++)
            {
                ret.Add(objs[i].RootName, new FieldInformations(objs[i].Fields));
            }

            return ret;
        }

        public static SetSettingsCommand CreateSettingsCommand<T>(string rootName, params T[] o)
            where T : class
        {
            return new SetSettingsCommand(Create(Create(rootName, o)));
        }

        public static SetSettingsCommand CreateSettingsCommand<T>(string rootName, string[] keys, params T[] o)
            where T : class
        {
            return new SetSettingsCommand(Create(Create(rootName, o)), keys);
        }

        public static ObjectFieldContainer Create<T>(string rootName, params T[] o)
            where T : class
        {
            Dictionary<string, FieldInformation> ret = new Dictionary<string, FieldInformation>();
            for (int i = 0; i < o.Length; i++)
            {
                Dictionary<string, FieldInformation> sub = GenerateFieldInformation(rootName, o[i]);
                foreach (KeyValuePair<string, FieldInformation> fieldInformation in sub)
                {
                    ret.Add(fieldInformation.Key, fieldInformation.Value);
                }
            }

            return new ObjectFieldContainer(rootName, ret);
        }

        public struct ObjectFieldContainer
        {

            public string RootName;
            public Dictionary<string, FieldInformation> Fields;

            public ObjectFieldContainer(string rootName, Dictionary<string, FieldInformation> fields)
            {
                Fields = fields;
                RootName = rootName;
            }

        }

    }
}