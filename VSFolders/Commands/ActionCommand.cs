using System;
using System.Windows.Input;

namespace Microsoft.VSFolders.ViewModels
{
    public class ActionCommand : ICommand
    {
        private readonly Action<object> _action;
        private readonly Action _actionParamless;
        private bool _canExecute;
        public void SetCanExecute(bool val)
        {
            _canExecute = val;
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, new EventArgs());
        }

        public ActionCommand(Action action, bool canExecute = true)
        {
            _actionParamless = action;
            _canExecute = canExecute;
        }

        public ActionCommand(Action<object> action, bool canExecute = true)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public void Execute(object parameter)
        {
            if (_actionParamless != null)
            {
                _actionParamless();
                return;
            }
            _action(parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}