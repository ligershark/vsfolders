// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GestureService.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   GestureService.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders.Services
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Input;
    using Commands;
    using Models;

    public class GestureService
    {
        private readonly Dictionary<Key, Func<KeyEventArgs, bool>> _bindings =
            new Dictionary<Key, Func<KeyEventArgs, bool>>();

        public GestureService()
        {
            this._bindings.Add(Key.F2, this.F2);
            this._bindings.Add(Key.Delete, this.Delete);
            this._bindings.Add(Key.Enter, this.Enter);
        }

        public void HandleKeys(KeyEventArgs e)
        {
            Func<KeyEventArgs, bool> act;
            if (this._bindings.TryGetValue(e.Key, out act))
            {
                e.Handled = act(e);
            }
        }

        public bool Enter(KeyEventArgs e)
        {
            FileData current = Factory.Resolve<FolderService>().SelectedItem;
            if (current == null)
            {
                return false;
            }

            if (current.IsDirectory)
            {
                current.IsExpanded = !current.IsExpanded;
                return true;
            }

            Factory.Resolve<OpenFileCommand>().Execute(current.FullPath);

            return true;
        }

        public bool Delete(KeyEventArgs e)
        {
            FileData current = Factory.Resolve<FolderService>().SelectedItem;
            if (current == null)
            {
                return false;
            }

            if (current.Parent == null)
            {
                Factory.Resolve<RemoveFolderCommand>().Execute(current);
            }
            else
            {
                Factory.Resolve<DeleteCommand>().Execute(current);
            }

            return true;
        }

        private bool F2(KeyEventArgs e)
        {
            FileData current = Factory.Resolve<FolderService>().SelectedItem;
            if (current == null)
            {
                return false;
            }

            Factory.Resolve<RenameCommand>().Execute(current);
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