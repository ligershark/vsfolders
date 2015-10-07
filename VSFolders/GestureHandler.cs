using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.VSFolders.Commands;
using Microsoft.VSFolders.Models;
using Microsoft.VSFolders.ViewModels;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;

namespace Microsoft.VSFolders
{
    public class GestureHandler
    {
        private Dictionary<Key, Func<KeyEventArgs, bool>> _bindings = new Dictionary<Key, Func<KeyEventArgs, bool>>();
        public FoldersWindowViewModel ViewModel { get; set; }

        public GestureHandler(FoldersWindowViewModel viewModel)
        {
            ViewModel = viewModel;
            _bindings.Add(Key.F2, F2);
            _bindings.Add(Key.Delete, Delete);
            _bindings.Add(Key.Enter, Enter);
            _bindings.Add(Key.B, Build);

        }

        public void HandleKeys(KeyEventArgs e)
        {
            Func<KeyEventArgs, bool> act;
            if (!_bindings.TryGetValue(e.Key, out act))
                return;
            e.Handled = act(e);
        }

        private bool Build(KeyEventArgs e)
        {
            var current = ViewModel.Current;
            if (current == null)
                return false;
            if ((e.GetModifier(Key.LeftCtrl) || e.GetModifier(Key.RightCtrl)) && (e.GetModifier(Key.LeftShift) || e.GetModifier(Key.RightShift)))
            {
                new BuildCommand().Execute(current);
                return true;
            }
            return false;
        }

        private bool F2(KeyEventArgs e)
        {
            var current = ViewModel.Current;
            if (current == null)
                return false;

            new RenameCommand().Execute(current);
            return true;
        }


        public bool Enter(KeyEventArgs e)
        {
            var current = ViewModel.Current;
            if (current == null)
                return false;

            if (!File.Exists(current.FullPath))
            {
                current.IsExpanded = !current.IsExpanded;
                return true;
            }

            var path = current.FullPath;

            try
            {
                if (File.Exists(path) && new FileInfo(path).Length > 10 * 1024 * 1024)
                {
                    if (MessageBox.Show("The file " + current.Name + " is pretty big and might take quite a while to open.\r\nAre you sure you want to open it?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) != MessageBoxResult.Yes)
                    {
                        return true;
                    }
                }

                VSFoldersPackage.DTE.ItemOperations.OpenFile(path);
            }
            catch
            {

            }

            return true;
        }
        public bool Delete(KeyEventArgs e)
        {
            var current = ViewModel.Current;
            if (current == null)
                return false;
            if (current.Parent == null)
            {
                new RemoveCommand().Execute(ViewModel.Current);
            }
            else
            {
                new DeleteCommand().Execute(ViewModel.Current);
            }
            return true;
        }
    }

    internal static class KeyEventExtensions
    {
        public static bool GetModifier(this KeyEventArgs e, Key modifier)
        {
            return e.KeyboardDevice.IsKeyDown(modifier);
        }
    }
}
