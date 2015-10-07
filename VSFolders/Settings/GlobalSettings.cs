using System.IO;
using Newtonsoft.Json.Linq;

namespace Microsoft.VSFolders.Settings
{
    public static class GlobalSettings
    {
        private static T LoadSettings<T>()
            where T : ISettingsObject<T>, new()
        {
            if (!File.Exists(SettingsStore.FileName))
            {
                return new T();
            }

            var text = File.ReadAllText(SettingsStore.FileName);
            var obj = JObject.Parse(text);

            JToken match;
            if (!obj.TryGetValue(typeof (T).FullName, out match))
            {
                return new T();
            }

            return match.ToObject<T>();
        } 

        public static FtpSettings AzureWebsites
        {
            get { return LoadSettings<FtpSettings>(); }
        }

        public static GeneralSettings General
        {
            get { return LoadSettings<GeneralSettings>(); }
        }

        public static ConsoleDefinitionCollection Console
        {
            get { return LoadSettings<ConsoleDefinitionCollection>(); }
        }
    }
}