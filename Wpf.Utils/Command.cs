using System;
using System.Windows.Input;

namespace Wpf.Utils
{
    public class Command : ICommand
    {
        private readonly Action<object> _handler;

        public Command(Action<object> handler)
        {
            _handler = handler;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _handler(parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}