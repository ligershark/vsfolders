using System.ComponentModel;
using Newtonsoft.Json;

namespace Microsoft.VSFolders.Settings
{
    public class AzureWebsitesObject : ISettingsObject<AzureWebsitesObject>
    {
        [JsonProperty]
        [DisplayName("User name")]
        [Description("The FTP username.")]
        [Category("Credential")]
        public string FtpUserName { get; set; }

        [JsonProperty]
        [DisplayName("Password")]
        [Description("The FTP password.")]
        [Category("Credential")]
        public string FtpPassword { get; set; }

        [JsonProperty]
        [DisplayName("Site Name")]
        [Description("A friendly name for the website.")]
        [Category("(General)")]
        public string FriendlyName { get; set; }

        public void ResetValues()
        {
            FtpUserName = null;
            FtpPassword = null;
            FriendlyName = null;
        }

        public void AssignFrom(AzureWebsitesObject other)
        {
            FtpUserName = other.FtpUserName;
            FtpPassword = other.FtpPassword;
            FriendlyName = other.FriendlyName;
        }

        public ISettingsObject<AzureWebsitesObject> CreateCopy()
        {
            return new AzureWebsitesObject
            {
                FriendlyName = FriendlyName,
                FtpPassword = FtpPassword,
                FtpUserName = FtpUserName
            };
        }
    }
}