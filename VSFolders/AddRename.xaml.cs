using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Microsoft.VSFolders.Models;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace Microsoft.VSFolders
{
    /// <summary>
    /// Interaction logic for AddRename.xaml
    /// </summary>
    public partial class AddRename : Window
    {
        private bool _activated;
        public AddRename()
        {
            InitializeComponent();
            ViewModel = new AddRenameViewModel();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (!_activated) // we have to do this here because we need to be sure binding is completed before SelectAll
            {
                this.txtInput.SelectAll();
                this.txtInput.Focus();
                _activated = true;
            }
        }

        public AddRenameViewModel ViewModel
        {
            get { return DataContext as AddRenameViewModel; }
            set { DataContext = value; }
        }

        public static DialogResult ShowAsDialog(string title, string path, out string fileName)
        {
            var dlg = new AddRename();
            dlg.ViewModel.FileName = Path.GetFileName(path);
            dlg.Title = title;

            var res = dlg.ShowDialog();
            fileName = null;

            if (res.GetValueOrDefault())
            {
                fileName = dlg.ViewModel.FileName;
                return System.Windows.Forms.DialogResult.OK;
            }
            return System.Windows.Forms.DialogResult.Cancel;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            CloseDialog();
        }

        private void CloseDialog()
        {
          if (String.IsNullOrEmpty(ViewModel.FileName))
                return;
            DialogResult = true;
        } 

        private void TxtInput_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Enter)
            {
                this.okButton.Focus();
                e.Handled = true;

                CloseDialog();
            }
        }
    }
}
