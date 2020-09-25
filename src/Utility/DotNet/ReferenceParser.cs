using System.Collections.Generic;
using System.Xml;

using Utility.DotNet.ProjectParsing;
using Utility.Serialization;

namespace Utility.DotNet
{
    internal static class ReferenceParser
    {

        public static CSharpReference Parse(XmlNode node)
        {
            CSharpReference ret = new CSharpReference();

            if (node.Name == "PackageReference")
            {
                ret.ReferenceType = CSharpReferenceType.PackageReference;
            }

            if (node.Name == "ProjectReference")
            {
                ret.ReferenceType = CSharpReferenceType.ProjectReference;
            }

            if (node.Name == "EmbeddedResource")
            {
                ret.ReferenceType = CSharpReferenceType.EmbeddedResource;
            }

            ret.internalAttributes = new List<SerializableKeyValuePair<string, string>>();
            List<XmlAttribute> attribs = CSharpProject.GetAttributes(node);

            foreach (XmlAttribute xmlAttribute in attribs)
            {
                ret.internalAttributes.Add(
                                           new SerializableKeyValuePair<string, string>
                                           {
                                               Key = xmlAttribute.Name,
                                               Value = xmlAttribute.Value
                                           }
                                          );
            }

            return ret;
        }

    }
}