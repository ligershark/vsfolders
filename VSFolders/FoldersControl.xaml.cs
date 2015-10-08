// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FoldersControl.xaml.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.VSFolders
{
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Commands;
    using Models;
    using Services;

    /// <summary>
    ///     Interaction logic for MyControl.xaml
    /// </summary>
    public partial class FoldersControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FoldersControl"/> class.
        /// </summary>
        public FoldersControl()
        {
            this.InitializeComponent();
            this.ViewModel = new FoldersWindowViewModel();
            Factory.Resolve<FolderNavigationService>().TreeView = this.Folders;
        }

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        public FoldersWindowViewModel ViewModel
        {
            get { return this.DataContext as FoldersWindowViewModel; }
            set { this.DataContext = value; }
        }

        /// <summary>
        /// The collapse all button click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CollapseAllButtonClick(object sender, RoutedEventArgs e)
        {
            foreach (TreeViewItem node in VisualTreeHelperHelper.FindVisualChildren<TreeViewItem>(this.Folders))
            {
                node.IsExpanded = false;
            }
        }

        /// <summary>
        /// The control_ on mouse double click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            TreeView tv = (TreeView)sender;
            this.ViewModel.OpenItemCommand.Execute(tv.SelectedItem as FileData);
        }

        /// <summary>
        /// The ui element_ on key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            Factory.Resolve<GestureService>().HandleKeys(e);
        }

        /// <summary>
        /// The folders_ on mouse down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Folders_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Folders.Focus();
        }

        /// <summary>
        /// The ui element_ on drag over.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void UIElement_OnDragOver(object sender, DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        /// <summary>
        /// The ui element_ on drop.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void UIElement_OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string file in files)
                {
                    if (Directory.Exists(file))
                    {
                        new AddFolderCommand().Execute(file);
                    }
                }
            }
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount != 1)
            {
                return;
            }

            FrameworkElement f = (FrameworkElement)sender;
            this.ViewModel.PreviewItemCommand.Execute(f.DataContext as FileData);
        }
    }
}