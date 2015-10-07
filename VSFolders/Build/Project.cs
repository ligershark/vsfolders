using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.VSFolders.Build
{
    public class Project : XmlObject
    {
        public Project()
        {
            Children = new List<XmlObject>();
        }

        [XmlAttribute]
        public string ToolsVersion { get { return Get(); } set { Set(value); } }
        [XmlAttribute]
        public string DefaultTargets { get { return Get(); } set { Set(value); } }

        public List<XmlObject> Children { get; set; }

        public void Add(XmlObject obj)
        {
            Children.Add(obj);
        }

        protected internal override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(GetType().Name, "http://schemas.microsoft.com/developer/msbuild/2003");
            foreach (var value in Properties)
            {
                writer.WriteAttributeString(value.Key, value.Value);
            }
            //writer.WriteNmToken(@"http://schemas.microsoft.com/developer/msbuild/2003");
            //writer.WriteAttributeString("xmlns", @"http://schemas.microsoft.com/developer/msbuild/2003");
            foreach (var child in Children)
                child.WriteXml(writer);
            WriteCloseXml(writer);
        }

       
    }
}
