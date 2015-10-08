// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObservableType.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   ObservableType.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders.Models
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class ObservableType : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void Set<T>(ref T local, T value, [CallerMemberName] string member = null)
        {
            local = value;
            this.OnPropertyChanged(member);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}