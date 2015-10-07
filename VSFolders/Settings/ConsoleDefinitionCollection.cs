using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;

namespace Microsoft.VSFolders.Settings
{
    public class ConsoleDefinitionCollection : ISettingsObject<ConsoleDefinitionCollection>
    {
        [JsonProperty]
        [DisplayName("Consoles")]
        [Description("Available Consoles.")]
        [Category("(General)")]
        public List<ConsoleDefinition> Consoles { get; set; }

        public void ResetValues()
        {
            Consoles.Clear();
        }

        public void AssignFrom(ConsoleDefinitionCollection other)
        {
            Consoles = (other.Consoles ?? new List<ConsoleDefinition>()).Select(x => (ConsoleDefinition) x.CreateCopy()).ToList();
        }

        public ISettingsObject<ConsoleDefinitionCollection> CreateCopy()
        {
            return new ConsoleDefinitionCollection
            {
                Consoles = (Consoles ?? new List<ConsoleDefinition>()).Select(x => (ConsoleDefinition) x.CreateCopy()).ToList()
            };
        }
    }
}