namespace Microsoft.VSFolders.Settings
{
    public class NoSettings : ISettingsObject<NoSettings>
    {
        public void ResetValues()
        {
        }

        public void AssignFrom(NoSettings other)
        {
        }

        public virtual ISettingsObject<NoSettings> CreateCopy()
        {
            return new NoSettings();
        }
    }
}