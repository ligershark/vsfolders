namespace Microsoft.VSFolders.Settings
{
    public class FtpSettingsPage : SettingsOptionPage<FtpSettings>
    {
        public FtpSettingsPage()
            : base(GlobalSettings.AzureWebsites)
        {
        }
    }
}