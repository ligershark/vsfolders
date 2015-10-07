namespace Microsoft.VSFolders.Settings
{
    public class ConsoleSettingsPage : SettingsOptionPage<ConsoleDefinitionCollection>
    {
        public ConsoleSettingsPage()
            : base(GlobalSettings.Console)
        {
        }
    }
}