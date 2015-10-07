using System;
using System.Windows.Input;

namespace Microsoft.VSFolders.Commands
{
    public abstract class CommandBase : ICommand
    {
        protected bool _canExecute;

        protected CommandBase(bool canExecute = true)
        {
            _canExecute = canExecute;
        }

        public virtual void SetCanExecute(bool val)
        {
            _canExecute = val;
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, new EventArgs());
        }

        public virtual bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public abstract void Execute(object parameter);

        public event EventHandler CanExecuteChanged;
    }
}