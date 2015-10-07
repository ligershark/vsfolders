using System.Xml;

namespace Microsoft.VSFolders.Build
{
    public class Conditional : Property
    {
        public Conditional()
        {

        }
        public Conditional(string name, string value, string condition)
            : base(name, value)
        {
            Condition = condition;
        }
        public string Condition { get { return Get(); } set { Set(value); } }
        protected internal override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(Name);
            writer.WriteAttributeString("Condition", Condition);
            writer.WriteString(Value);
            writer.WriteEndElement();
        }
    }
}