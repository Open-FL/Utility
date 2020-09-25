using System.Collections.Generic;
using System.Xml.Serialization;

using Utility.Serialization;

namespace Utility.DotNet.ProjectParsing
{
    public struct CSharpReference
    {

        [XmlElement(ElementName = "Attributes")]
        public List<SerializableKeyValuePair<string, string>> internalAttributes;

        [XmlIgnore]
        public Dictionary<string, string> Attributes
        {
            get
            {
                Dictionary<string, string> ret = new Dictionary<string, string>();
                internalAttributes.ForEach(x => ret.Add(x.Key, x.Value));
                return ret;
            }
        }

        public CSharpReferenceType ReferenceType;

    }
}