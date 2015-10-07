namespace Microsoft.VSFolders.Build
{
    public class Reference : XmlObject
    {
        public Reference(string include)
        {
            Include = include;
        }
        public string Include { get { return Get(); } set { Set(value); } }
    }
}