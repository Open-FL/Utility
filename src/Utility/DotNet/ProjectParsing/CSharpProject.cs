using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Utility.DotNet.ProjectParsing
{
    public class CSharpProject
    {

        private readonly XmlDocument document;

        internal CSharpProject(XmlDocument document)
        {
            this.document = document;
        }


        public XmlNode ProjectNode => document.FirstChild;

        public List<CSharpReference> References => ParseReferences(GetChildren(ProjectNode).ToArray());

        public List<CSharpReference> ProjectReferences =>
            References.Where(x => x.ReferenceType == CSharpReferenceType.ProjectReference).ToList();

        public List<CSharpReference> EmbeddedReferences =>
            References.Where(x => x.ReferenceType == CSharpReferenceType.EmbeddedResource).ToList();

        public List<CSharpReference> PackageReferences =>
            References.Where(x => x.ReferenceType == CSharpReferenceType.PackageReference).ToList();

        public static List<XmlNode> GetChildren(XmlNode node)
        {
            List<XmlNode> ret = new List<XmlNode>();
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                ret.Add(node.ChildNodes[i]);
            }

            return ret;
        }

        public static List<XmlAttribute> GetAttributes(XmlNode node)
        {
            List<XmlAttribute> ret = new List<XmlAttribute>();
            for (int i = 0; i < node.Attributes.Count; i++)
            {
                ret.Add(node.Attributes[i]);
            }

            return ret;
        }

        private static List<CSharpReference> ParseReferences(XmlNode[] propertyGroups)
        {
            List<CSharpReference> ret = new List<CSharpReference>();
            for (int i = 0; i < propertyGroups.Length; i++)
            {
                List<XmlNode> children = GetChildren(propertyGroups[i]);
                for (int childIndex = 0; childIndex < children.Count; childIndex++)
                {
                    ret.Add(ReferenceParser.Parse(children[childIndex]));
                }
            }

            return ret;
        }

        public void Save(string path)
        {
            document.Save(path);
        }

        public void AddReference(CSharpReference reference)
        {
            XmlNode container = document.CreateNode(XmlNodeType.Element, "ItemGroup", "");
            XmlNode node = document.CreateNode(XmlNodeType.Element, reference.ReferenceType.ToString(), "");
            for (int i = 0; i < reference.internalAttributes.Count; i++)
            {
                XmlAttribute a = document.CreateAttribute(reference.internalAttributes[i].Key);
                a.Value = reference.internalAttributes[i].Value;
                node.Attributes.Append(a);
            }

            container.AppendChild(node);
            ProjectNode.AppendChild(container);
        }

    }
}