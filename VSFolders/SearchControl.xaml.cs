// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchControl.xaml.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   SearchControl.xaml.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders
{
    using System.Windows.Controls;
    using System.Windows.Input;
    using Models;

    /// <summary>
    /// Interaction logic for SearchControl.xaml
    /// </summary>
    public partial class SearchControl : UserControl
    {
        public SearchControl()
        {
            this.InitializeComponent();
            this.ViewModel = new SearchControlViewModel();
        }

        public SearchControlViewModel ViewModel
        {
            get { return this.DataContext as SearchControlViewModel; }
            set { this.DataContext = value; }
        }

        private void SearchTextPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.ViewModel.SearchText = string.Empty;
                e.Handled = true;
            }

            this.SearchText.Focus();
        }
    }
}