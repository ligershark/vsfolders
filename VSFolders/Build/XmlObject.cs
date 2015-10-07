using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Microsoft.VSFolders.Build
{
    public abstract class XmlObject
    {
        protected readonly Dictionary<string, string> Properties = new Dictionary<string, string>();

        public string this[string key]
        {
            get
            {
                string obj;
                Properties.TryGetValue(key, out obj);
                return obj;
            }
            set { Properties[key] = value; }
        }

        protected string Get([CallerMemberName] string caller = null)
        {
            return this[caller];
        }

        protected void Set(string value, [CallerMemberName] string caller = null)
        {
            this[caller] = value;
        }

        protected internal virtual void WriteOpenXml(XmlWriter writer)
        {
            writer.WriteStartElement(GetType().Name);
            foreach (var value in Properties)
            {
                writer.WriteAttributeString(value.Key, value.Value);
            }
        }

        protected internal virtual void WriteCloseXml(XmlWriter writer)
        {
            writer.WriteEndElement();
        }

        protected internal virtual void WriteXml(XmlWriter writer)
        {
            WriteOpenXml(writer);
            WriteCloseXml(writer);
        }

        public string GetXml()
        {
            var settings = new XmlWriterSettings {Indent = true, Async = false};
            using (var stream = new StringWriter())
            using (var writer = XmlWriter.Create(stream, settings))
            {
                WriteXml(writer);
                writer.Flush();
                writer.Close();
                return stream.ToString();
            }
        }
    }
}