using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.VSFolders.Settings;
using Newtonsoft.Json;

namespace Microsoft.VSFolders.Console
{
    public class ConsoleDefinition : INotifyPropertyChanged
    {
        public static ConsoleDefinition From(IConsoleDefinition def)
        {
            return new ConsoleDefinition
            {
                Separator = def.Separator,
                Arguments = def.Arguments,
                Executable = def.Executable,
                ExitCommand = def.ExitCommand,
                ExitDelay = def.ExitDelay
            };
        }

        private string _arguments;
        private string _executable;
        private string _separator = "\\";
        private string _exitCommand;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Separator
        {
            get { return _separator; }
            set
            {
                _separator = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty]
        public string Executable
        {
            get { return _executable; }
            set
            {
                _executable = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Arguments
        {
            get { return _arguments; }
            set
            {
                _arguments = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ExitCommand
        {
            get { return _exitCommand; }
            set
            {
                _exitCommand = value;
                OnPropertyChanged();
            }
        }

        private int _exitDelay;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int ExitDelay
        {
            get { return _exitDelay; }
            set
            {
                _exitDelay = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}