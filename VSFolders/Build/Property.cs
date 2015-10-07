using System.Xml;

namespace Microsoft.VSFolders.Build
{
    public class Property : XmlObject
    {
        public Property() { }
        public Property(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get { return Get(); } set { Set(value); } }
        public string Value { get { return Get(); } set { Set(value); } }

        public static implicit operator Property(string value)
        {
            return new Property { Value = value };
        }

        protected internal override void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString(Name, Value);
        }
    }
}