namespace Microsoft.VSFolders.Build
{
    public class None : XmlObject
    {
        public None(string include)
        {
            Include = include;
        }
        public string Include { get { return Get(); } set { Set(value); } }
    }
}