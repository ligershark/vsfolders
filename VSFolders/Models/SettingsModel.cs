using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.VSFolders.Settings;
using Newtonsoft.Json;
using ConsoleDefinition = Microsoft.VSFolders.Console.ConsoleDefinition;

namespace Microsoft.VSFolders
{
    public class SettingsModel : INotifyPropertyChanged
    {
        private IReadOnlyDictionary<string, ConsoleDefinition> _consoles;
        private string _defaultConsole;
        private bool _previewItems;
        private bool _searchInFiles;
        private bool _showHiddenFiles;

        [JsonIgnore]
        public IReadOnlyDictionary<string, ConsoleDefinition> Consoles
        {
            get { return _consoles; }
            set
            {
                _consoles = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty]
        public ObservableCollection<string> OpenFolders { get; set; }

        [JsonProperty]
        public ObservableCollection<string> ExpandedFolders { get; set; }

        [JsonProperty]
        public string DefaultConsole
        {
            get { return _defaultConsole; }
            set
            {
                _defaultConsole = value;
                OnPropertyChanged();
            }
        }

        public SettingsModel()
        {
            VSFoldersPackageOnSettingsChanged(null, null);
            VSFoldersPackage.SettingsChanged += VSFoldersPackageOnSettingsChanged;
        }

        private void VSFoldersPackageOnSettingsChanged(object sender, EventArgs eventArgs)
        {
            var target = VSFoldersPackage.GetDialogPage<ConsoleSettingsPage>().OriginalTarget;
            var defaultConsole = DefaultConsole;

            if (target == null)
            {
                return;
            }

            if (target.Consoles != null)
            {
                if (target.Consoles.Count > 0 && target.Consoles.All(x => x.Name == null))
                {
                    return;
                }

                Consoles = target.Consoles == null || target.Consoles.Count == 0
                    ? DefaultConsoles
                    : target.Consoles.ToDictionary(x => (x.Name ?? ("(Unknown Name) - " + x.Executable)), ConsoleDefinition.From);
            }
            else
            {
                Consoles = DefaultConsoles;
                DefaultConsole = string.Empty;
                return;
            }

            ConsoleDefinition console;
            if (defaultConsole != null && !Consoles.TryGetValue(defaultConsole, out console))
            {
                var tmp = Consoles.FirstOrDefault();
                DefaultConsole = tmp.Key;
            }
            else
            {
                DefaultConsole = defaultConsole;
            }
        }

        private static readonly IReadOnlyDictionary<string, ConsoleDefinition> DefaultConsoles = new Dictionary<string, ConsoleDefinition>
        {
            {
                "Command Prompt", new ConsoleDefinition
                {
                    Executable = "cmd",
                    ExitCommand = "exit",
                    ExitDelay = 1000,
                    Separator = "\\"
                }
            }
        };

        [JsonProperty]
        public bool PreviewItems
        {
            get { return _previewItems; }
            set
            {
                _previewItems = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public bool SuppressSave { get; set; }

        [JsonProperty]
        public bool SearchInFiles
        {
            get { return _searchInFiles; }
            set
            {
                _searchInFiles = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty]
        public bool ShowHiddenFiles
        {
            get { return _showHiddenFiles; }
            set
            {
                _showHiddenFiles = value;
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