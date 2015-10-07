using System;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.VSFolders.Settings
{
    public abstract class SettingsOptionPage<T> : DialogPage
        where T : ISettingsObject<T>
    {
        ///<summary>Gets the actual settings instance read by the application.</summary>
        public T OriginalTarget { get; private set; }
        ///<summary>Gets a clone of the settings instance used to bind the options pages.</summary>
        public T DialogTarget { get; private set; }
        public override object AutomationObject { get { return DialogTarget; } }

        protected SettingsOptionPage(T target)
        {
            OriginalTarget = target;
            DialogTarget = (T) target.CreateCopy();
            //TODO: Update copy when dialog is shown?
            //Settings.Updated += delegate { LoadSettingsFromStorage(); };
        }

        public override void ResetSettings()
        {
            DialogTarget.ResetValues();
        }

        public override void LoadSettingsFromStorage()
        {
            DialogTarget.AssignFrom(OriginalTarget);
        }
        public override void SaveSettingsToStorage()
        {
            OriginalTarget.AssignFrom(DialogTarget);
            SettingsStore.SaveSettings(DialogTarget);
        }
    }
}
