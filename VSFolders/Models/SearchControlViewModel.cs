// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchControlViewModel.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   SearchControlViewModel.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders.Models
{
    using System.Windows.Input;
    using Commands;
    using Search;

    public class SearchControlViewModel : ObservableType
    {
        private string _searchText;

        public SearchControlViewModel()
        {
            this.SearchProvider = Factory.Resolve<SearchProvider>();

            // PerformSearchCommand = new ActionCommand(o => SearchProvider.SearchInFiles(this.SearchText));
            this.ClearSearchCommand = new ActionCommand(o => this.SearchText = string.Empty);
        }

        public ICommand ClearSearchCommand { get; private set; }

        public ICommand PerformSearchCommand { get; private set; }

        public SearchProvider SearchProvider { get; set; }

        public bool IsSearching
        {
            get { return !string.IsNullOrEmpty(this.SearchText); }
        }

        public string SearchText
        {
            get { return this._searchText; }
            set
            {
                // DataSource.Tree.ExpandOnFilterMatch = !string.IsNullOrEmpty(value);
                this._searchText = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged("IsSearching");

                // DataSource.Tree.ApplyFilter(n => Filter(n.Value));
            }
        }
    }
}