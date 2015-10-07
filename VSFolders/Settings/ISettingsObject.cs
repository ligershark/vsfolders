namespace Microsoft.VSFolders.Settings
{
    public interface ISettingsObject<in T>
        where T : ISettingsObject<T>
    {
        void ResetValues();

        void AssignFrom(T other);

        ISettingsObject<T> CreateCopy();
    }
}