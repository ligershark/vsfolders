using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;

namespace Microsoft.VSFolders.Settings
{
    public class FtpSettings : ISettingsObject<FtpSettings>
    {
        [JsonProperty]
        [Category("FTP Publish")]
        [DisplayName("Azure Websites")]
        [Description("Azure websites configured for quick FTP publishing.")]
        public List<AzureWebsitesObject> Sites { get; set; }
        
        public void ResetValues()
        {
            Sites.Clear();
        }

        public void AssignFrom(FtpSettings other)
        {
            if (other.Sites != null)
            {
                Sites = other.Sites.Select(x => (AzureWebsitesObject) x.CreateCopy()).ToList();
            }
            else
            {
                Sites = new List<AzureWebsitesObject>();
            }
        }

        public ISettingsObject<FtpSettings> CreateCopy()
        {
            return new FtpSettings
            {
                Sites = (Sites ?? new List<AzureWebsitesObject>()).Select(x => (AzureWebsitesObject) x.CreateCopy()).ToList()
            };
        }
    }
}