// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Settings.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   Settings.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders
{
    using System.Collections.ObjectModel;
    using System.IO;
    using Models;
    using Newtonsoft.Json;

    public class Settings : ObservableType
    {
        private bool _previewItems;

        private bool _searchInFiles;

        private bool _showHiddenFiles;

        public static Settings Instance { get { return Factory.Resolve<Settings>(); } }

        [JsonProperty]
        public ObservableCollection<string> OpenFolders { get; set; }

        [JsonProperty]
        public bool PreviewItems
        {
            get { return this._previewItems; }
            set
            {
                this.Set(ref this._previewItems, value);
            }
        }

        [JsonIgnore]
        public bool SuppressSave { get; set; }

        [JsonProperty]
        public bool SearchInFiles
        {
            get { return this._searchInFiles; }
            set
            {
                this.Set(ref this._searchInFiles, value);
            }
        }

        [JsonProperty]
        public bool ShowHiddenFiles
        {
            get { return this._showHiddenFiles; }
            set
            {
                this.Set(ref this._showHiddenFiles, value);
            }
        }

        public static Settings LoadSettings(string path)
        {
            string file = path;
            string settingsText = File.Exists(file) ? File.ReadAllText(file) : "{}";
            var settings = JsonConvert.DeserializeObject<Settings>(settingsText);
            return settings;
        }

        public void Save(string path)
        {
            var settings = Factory.Resolve<Settings>();
            if (!settings.SuppressSave)
            {
                File.WriteAllText(path, JsonConvert.SerializeObject(Factory.Resolve<Settings>()));
            }
        }
    }
}