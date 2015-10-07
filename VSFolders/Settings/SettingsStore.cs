using System.IO;
using Newtonsoft.Json.Linq;

namespace Microsoft.VSFolders.Settings
{
    public static class SettingsStore
    {
        internal const string FileName = "vsfolders.settings.json";

        public static void SaveSettings<T>(T value)
            where T : ISettingsObject<T>
        {
            string text;

            if (!File.Exists(FileName))
            {
                text = "{\"Microsoft.VSFolders.Settings.GeneralSettings\":{\"AlwaysHidePatterns\":[{\"Name\":\"*.dll\"},{\"Name\":\"*\\\\bin\\\\*\"},{\"Name\":\"*\\\\obj\\\\*\"},{\"Name\":\"*\\\\bin\"},{\"Name\":\"*\\\\obj\"},{\"Name\":\"*.*.user\"}]},\"Microsoft.VSFolders.Settings.ConsoleDefinitionCollection\":{\"Consoles\":[{\"Name\":\"Command Prompt\",\"Separator\":\"\\\\\",\"Executable\":\"cmd\",\"Arguments\":\"\",\"ExitCommand\":\"exit\",\"ExitDelay\":1000},{\"Name\":\"Powershell\",\"Separator\":\"\\\\\",\"Executable\":\"powershell\",\"Arguments\":\"\",\"ExitCommand\":\"exit\",\"ExitDelay\":1000}]}}";
            }
            else
            {
                text = File.ReadAllText(FileName);
            }

            var inst = JObject.Parse(text);
            inst[typeof (T).FullName] = JObject.FromObject(value);
            File.WriteAllText(FileName, inst.ToString());
            VSFoldersPackage.FireSettingsChanged();
        }
    }
}