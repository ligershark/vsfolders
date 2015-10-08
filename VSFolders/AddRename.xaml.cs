// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddRename.xaml.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   AddRename.xaml.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders
{
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Input;
    using Models;
    using KeyEventArgs = System.Windows.Input.KeyEventArgs;

    /// <summary>
    /// Interaction logic for AddRename.xaml
    /// </summary>
    public partial class AddRename : Window
    {
        private bool _activated;

        public AddRename()
        {
            this.InitializeComponent();
            this.ViewModel = new AddRenameViewModel();
        }

        public AddRenameViewModel ViewModel
        {
            get { return this.DataContext as AddRenameViewModel; }
            set { this.DataContext = value; }
        }

        public static DialogResult ShowAsDialog(string title, string path, out string fileName)
        {
            AddRename dlg = new AddRename
            {
                ViewModel = {FileName = Path.GetFileName(path)},
                Title = title
            };

            bool? res = dlg.ShowDialog();
            fileName = null;

            if (res.GetValueOrDefault())
            {
                fileName = dlg.ViewModel.FileName;
                return System.Windows.Forms.DialogResult.OK;
            }

            return System.Windows.Forms.DialogResult.Cancel;
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (!this._activated)
            {
// we have to do this here because we need to be sure binding is completed before SelectAll
                this.txtInput.SelectAll();
                this.txtInput.Focus();
                this._activated = true;
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            this.CloseDialog();
        }

        private void CloseDialog()
        {
            if (string.IsNullOrEmpty(this.ViewModel.FileName))
            {
                return;
            }

            this.DialogResult = true;
        }

        private void TxtInput_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Enter)
            {
                this.okButton.Focus();
                e.Handled = true;

                this.CloseDialog();
            }
        }
    }
}