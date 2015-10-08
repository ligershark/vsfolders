// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddRenameViewModel.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   AddRenameViewModel.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders.Models
{
    using VisualStudio.PlatformUI;

    public class AddRenameViewModel : ObservableObject
    {
        private string _title;

        private string _fileName;

        public string Title
        {
            get { return this._title; }
            set
            {
                this._title = value;
                this.NotifyPropertyChanged();
            }
        }

        public string FileName
        {
            get { return this._fileName; }
            set
            {
                this._fileName = value;
                this.NotifyPropertyChanged();
            }
        }
    }
}