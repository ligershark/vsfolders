namespace Microsoft.VSFolders.Settings
{
    public class GeneralSettingsPage : SettingsOptionPage<GeneralSettings>
    {
        public GeneralSettingsPage()
            : base(GlobalSettings.General)
        {
        }
    }
}