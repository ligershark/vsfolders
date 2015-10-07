using System.Collections.Generic;
using System.Xml;

namespace Microsoft.VSFolders.Build
{
    public class PropertyGroup : XmlObject
    {
        public PropertyGroup()
        {
            Children = new List<XmlObject>();
        }

        public List<XmlObject> Children { get; set; }

        public void Add(XmlObject obj)
        {
            Children.Add(obj);
        }

        protected internal override void WriteXml(XmlWriter writer)
        {
            WriteOpenXml(writer);
            foreach (var child in Children)
                child.WriteXml(writer);
            WriteCloseXml(writer);
        }
    }
}