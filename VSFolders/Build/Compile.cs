namespace Microsoft.VSFolders.Build
{
    public class Compile : XmlObject
    {
        public Compile(string include)
        {
            Include = include;
        }
        public string Include { get { return Get(); } set { Set(value); } }
    }
}