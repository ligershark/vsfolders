// --------------------------------------- -----------------------------------------------------------------------------
// <copyright file="ActionCommand.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//   ActionCommand.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.VSFolders.Commands
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// A command that executes a callback action
    /// </summary>
    public class ActionCommand : ICommand
    {
        /// <summary>
        /// The _action
        /// </summary>
        private readonly Action<object> _action;

        /// <summary>
        /// The _action paramless
        /// </summary>
        private readonly Action _actionParamless;

        /// <summary>
        /// The _can execute
        /// </summary>
        private bool _canExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionCommand"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="canExecute">if set to <c>true</c> [can execute].</param>
        public ActionCommand(Action action, bool canExecute = true)
        {
            this._actionParamless = action;
            this._canExecute = canExecute;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionCommand"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="canExecute">if set to <c>true</c> [can execute].</param>
        public ActionCommand(Action<object> action, bool canExecute = true)
        {
            this._action = action;
            this._canExecute = canExecute;
        }

        /// <summary>
        /// Occurs when [can execute changed].
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Sets the can execute.
        /// </summary>
        /// <param name="val">if set to <c>true</c> [value].</param>
        public void SetCanExecute(bool val)
        {
            this._canExecute = val;
            this.CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Determines whether this instance can execute with the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return this._canExecute;
        }

        /// <summary>
        /// Executes with the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        public void Execute(object parameter)
        {
            if (this._actionParamless != null)
            {
                this._actionParamless();
                return;
            }

            this._action(parameter);
        }
    }
}