namespace Microsoft.VSFolders
{
    using Commands;

    public static class GlobalCommands
    {
        public static NavigateToCommand NavigateTo
        {
            get { return Factory.Resolve<NavigateToCommand>(); }
        }

        public static SyncActiveDocumentCommand SyncActiveDocument
        {
            get { return Factory.Resolve<SyncActiveDocumentCommand>(); }
        }
    }
}
