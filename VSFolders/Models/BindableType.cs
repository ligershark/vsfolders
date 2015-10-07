using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Microsoft.VSFolders.Models
{
    public class BindableType : INotifyPropertyChanged
    {
        protected void Set<T>(ref T local, T value, [CallerMemberName] string member = null)
        {
            if (!Equals(local,value))
            {
                local = value;
                OnPropertyChanged(member);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
