using Microsoft.VisualStudio.PlatformUI;

namespace Microsoft.VSFolders.Models
{
    public class AddRenameViewModel : ObservableObject
    {
        private string _title;
        private string _fileName;

        public string Title { get { return _title; } set { _title = value; NotifyPropertyChanged(); } }

        public string FileName { get { return _fileName; } set { _fileName = value; NotifyPropertyChanged(); } }


    }
}
