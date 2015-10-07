using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace Microsoft.VSFolders.Settings
{
    public class WrappedString
    {
        [JsonProperty]
        [DisplayName("Name")]
        public string Name { get; set; }
    }

    public class GeneralSettings : ISettingsObject<GeneralSettings>
    {
        [JsonProperty]
        [DisplayName("Always Hide")]
        [Description("A set of file patterns to always hide.")]
        [Category("Folders")]
        public List<WrappedString> AlwaysHidePatterns { get; set; }

        public void ResetValues()
        {
            (AlwaysHidePatterns ?? (AlwaysHidePatterns = new List<WrappedString>())).Clear();
        }

        public void AssignFrom(GeneralSettings other)
        {
            AlwaysHidePatterns = other.AlwaysHidePatterns ?? new List<WrappedString>();
        }

        ISettingsObject<GeneralSettings> ISettingsObject<GeneralSettings>.CreateCopy()
        {
            return new GeneralSettings
            {
                AlwaysHidePatterns = AlwaysHidePatterns ?? new List<WrappedString>()
            };
        }
    }
}