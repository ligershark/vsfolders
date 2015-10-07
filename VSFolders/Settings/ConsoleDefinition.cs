using System.ComponentModel;
using Newtonsoft.Json;

namespace Microsoft.VSFolders.Settings
{
    public class ConsoleDefinition : IConsoleDefinition, ISettingsObject<ConsoleDefinition>
    {
        [JsonProperty]
        [DisplayName("Name")]
        [Description("The name of the console.")]
        [Category("(General)")]
        public string Name { get; set; }

        [JsonProperty]
        [DisplayName("Path Separator")]
        [Description("The path separator.")]
        [Category("Pathing")]
        public string Separator { get; set; }

        [JsonProperty]
        [DisplayName("Executable")]
        [Description("The path to the executable.")]
        [Category("Startup")]
        public string Executable { get; set; }
        
        [JsonProperty]
        [DisplayName("Command line arguments")]
        [Description("Arguments to supply when starting the executable.")]
        [Category("Startup")]
        public string Arguments { get; set; }
        
        [JsonProperty]
        [DisplayName("Exit Command")]
        [Description("The command to execute to finish the console session.")]
        [Category("Shutdown")]
        public string ExitCommand { get; set; }

        [JsonProperty]
        [DisplayName("Exit Delay (ms)")]
        [Description("How long to wait before killing the console process after attempting to exit.")]
        [Category("Shutdown")]
        public int ExitDelay { get; set; }

        public ConsoleDefinition()
        {
            ResetValues();
        }

        public void ResetValues()
        {
            Separator = "\\";
            Executable = "cmd";
            Arguments = string.Empty;
            ExitCommand = "exit";
            ExitDelay = 1000;
            Name = "Command Prompt";
        }

        public void AssignFrom(ConsoleDefinition other)
        {
            Name = other.Name;
            Separator = other.Separator;
            Executable = other.Executable;
            Arguments = other.Arguments;
            ExitDelay = other.ExitDelay;
            ExitCommand = other.ExitCommand;
        }

        public ISettingsObject<ConsoleDefinition> CreateCopy()
        {
            return new ConsoleDefinition
            {
                Name = Name,
                Separator = Separator,
                Executable = Executable,
                Arguments = Arguments,
                ExitDelay = ExitDelay,
                ExitCommand = ExitCommand
            };
        }
    }
}