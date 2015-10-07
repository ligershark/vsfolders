using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.VSFolders.FastTree;
using Microsoft.VSFolders.Models;
using Microsoft.VSFolders.ShellIcons;
using Microsoft.VSFolders.ViewModels;
using DataFormats = System.Windows.DataFormats;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;
using TreeView = System.Windows.Controls.TreeView;

namespace Microsoft.VSFolders
{
    /// <summary>
    ///     Interaction logic for MyControl.xaml
    /// </summary>
    public partial class FoldersControl
    {
        public FoldersControl()
        {
            InitializeComponent();
            ViewModel = new FoldersWindowViewModel();
        }

        public FoldersWindowViewModel ViewModel
        {
            get { return DataContext as FoldersWindowViewModel; }
            set { DataContext = value; }
        }

        private void CollapseAllButtonClick(object sender, RoutedEventArgs e)
        {
            foreach (var node in FindVisualChildren<TreeViewItem>(Folders))
            {
                node.IsExpanded = false;
            }
        }

        private void Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            var tv = (TreeView)sender;

            if (tv.SelectedItem == null)
            {
                return;
            }

            var dc = ((TreeNode<FileData>)tv.SelectedItem).Value;

            if (!File.Exists(dc.FullPath))
            {
                return;
            }

            var path = dc.FullPath;

            try
            {
                if (File.Exists(path) && new FileInfo(path).Length > 10 * 1024 * 1024)
                {
                    if (MessageBox.Show("The file " + dc.Name + " is pretty big and might take quite a while to open.\r\nAre you sure you want to open it?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) != MessageBoxResult.Yes)
                    {
                        return;
                    }
                }

                VSFoldersPackage.DTE.ItemOperations.OpenFile(path);
            }
            catch
            {
                try
                {
                    System.Diagnostics.Process.Start(path);
                }
                catch
                {
                }
            }
        }

        private void ShowInPreviewWindowIfSelected(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (ViewModel.IsUpdating)
            {
                return;
            }

            var tv = (TreeView)sender;

            try
            {
                if (tv.SelectedItem == null)
                {
                    return;
                }

                ViewModel.IsUpdating = true;
                var fullPath = ((TreeNode<FileData>)tv.SelectedItem).Value.FullPath;

                if (VSFoldersPackage.Settings.PreviewItems)
                {
                    if (File.Exists(fullPath) && new FileInfo(fullPath).Length < 1024 * 1024)
                    {
                        try
                        {
                            ShellHelpers.OpenFileInPreviewTab(fullPath);
                        }
                        catch
                        {
                        }
                        finally
                        {
                            tv.Focus();
                        }
                    }
                }

                ViewModel.Path = fullPath;
            }
            finally
            {
                ViewModel.IsUpdating = false;
            }
        }

        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            ViewModel.Gestures.HandleKeys(e);
        }

        private void SearchLostFocus(object sender, RoutedEventArgs e)
        {
            SearchOptions.IsOpen = false;
        }

        //private void RenameBoxLostFocus(object sender, RoutedEventArgs e)
        //{
        //    var fileData = (FileData)((FrameworkElement)sender).DataContext;
        //    fileData.CancelRenameCommand.Execute(null);
        //}

        private void ShowSearchOptionsClicked(object sender, RoutedEventArgs e)
        {
            SearchOptions.Width = SearchPane.ActualWidth;
            SearchOptions.HorizontalOffset = 0;
            SearchOptions.VerticalOffset = 0;
            SearchOptions.IsOpen = true;
            FirstOption.Focus();
        }

        private void Folders_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Folders.Focus();
        }

        private void UIElement_OnDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void UIElement_OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                
                foreach (string file in files)
                {
                    if (Directory.Exists(file))
                    {
                        ViewModel.AddPath(file);
                    }
                }
            }
        }

        private void SearchTextPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                ViewModel.SearchText = string.Empty;
                e.Handled = true;
            }

            SearchText.Focus();
        }

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    var item = child as T;
                    if (item != null)
                    {
                        yield return item;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
}