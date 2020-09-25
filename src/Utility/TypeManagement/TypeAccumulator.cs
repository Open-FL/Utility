using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Utility.TypeManagement
{
    public class TypeAccumulator<T> : TypeAccumulator
    {

        public new static List<Type> GetTypesByName(string name)
        {
            return GetAssignable(TypeAccumulator.GetTypesByName(name));
        }

        public new static List<Type> GetTypesByFullName(string fullName)
        {
            return GetAssignable(TypeAccumulator.GetTypesByFullName(fullName));
        }

        public new static Type GetTypeByAssemblyQualifiedName(string assemblyQualifiedName)
        {
            Type r = TypeAccumulator.GetTypeByAssemblyQualifiedName(assemblyQualifiedName);
            return typeof(T).IsAssignableFrom(r) ? r : null;
        }


        private static List<Type> GetAssignable(List<Type> types)
        {
            for (int i = 0; i < types.Count; i++)
            {
                Type assemblyTypeInfo = types[i];
                if (!typeof(T).IsAssignableFrom(assemblyTypeInfo))
                {
                    types.RemoveAt(i);
                }
            }

            return types;
        }

    }

    public class TypeAccumulator
    {

        private static readonly List<AssemblyTypeInfo> infos = new List<AssemblyTypeInfo>();

        protected TypeAccumulator()
        {
        }

        public static List<Type> GetTypesByName(string name)
        {
            List<Type> ret = new List<Type>();

            foreach (AssemblyTypeInfo assemblyTypeInfo in infos)
            {
                ret.AddRange(assemblyTypeInfo.GetTypesByName(name));
            }

            return ret;
        }

        public static List<Type> GetTypesByFullName(string fullName)
        {
            List<Type> ret = new List<Type>();

            foreach (AssemblyTypeInfo assemblyTypeInfo in infos)
            {
                ret.AddRange(assemblyTypeInfo.GetTypesByFullName(fullName));
            }

            return ret;
        }

        public static Type GetTypeByAssemblyQualifiedName(string assemblyQualifiedName)
        {
            foreach (AssemblyTypeInfo assemblyTypeInfo in infos)
            {
                Type r = assemblyTypeInfo.GetTypeByAssemblyQualifiedName(assemblyQualifiedName);
                if (r != null)
                {
                    return r;
                }
            }

            return null;
        }

        public static void RegisterAssembly(Assembly asm)
        {
            if (infos.Count(x => x.ContainingAssembly == asm) != 0)
            {
                return;
            }

            infos.Add(new AssemblyTypeInfo(asm));
        }


        protected class AssemblyTypeInfo
        {

            public AssemblyTypeInfo(Assembly asm)
            {
                ContainingAssembly = asm;
                ContainingTypes.AddRange(asm.GetExportedTypes());
            }

            public Assembly ContainingAssembly { get; }

            public List<Type> ContainingTypes { get; } = new List<Type>();

            public List<Type> GetTypesByName(string name)
            {
                List<Type> ret = new List<Type>();
                foreach (Type containingType in ContainingTypes)
                {
                    if (containingType.Name == name)
                    {
                        ret.Add(containingType);
                    }
                }

                return ret;
            }

            public List<Type> GetTypesByFullName(string fullName)
            {
                List<Type> ret = new List<Type>();
                foreach (Type containingType in ContainingTypes)
                {
                    if (containingType.FullName == fullName)
                    {
                        ret.Add(containingType);
                    }
                }

                return ret;
            }

            public Type GetTypeByAssemblyQualifiedName(string assemblyQualifiedName)
            {
                foreach (Type containingType in ContainingTypes)
                {
                    if (containingType.AssemblyQualifiedName == assemblyQualifiedName)
                    {
                        return containingType;
                    }
                }

                return null;
            }

        }

    }
}