// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandBase.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   CommandBase.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders.Commands
{
    using System;
    using System.Windows.Input;

    public abstract class CommandBase : ICommand
    {
        protected bool _canExecute;

        protected CommandBase(bool canExecute = true)
        {
            this._canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public virtual void SetCanExecute(bool val)
        {
            this._canExecute = val;
            this.CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        public virtual bool CanExecute(object parameter)
        {
            return this._canExecute;
        }

        public abstract void Execute(object parameter);
    }
}