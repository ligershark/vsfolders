using Newtonsoft.Json;

namespace Microsoft.VSFolders.Settings
{
    public interface IConsoleDefinition
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        string Separator { get; set; }

        [JsonProperty]
        string Executable { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        string Arguments { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        string ExitCommand { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        int ExitDelay { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        string Name { get; set; }
    }
}