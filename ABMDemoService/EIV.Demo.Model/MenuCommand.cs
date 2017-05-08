

namespace EIV.Demo.Model
{
    using System;
    using System.Windows.Input;

    public sealed class MenuCommand : ICommand
    {
        private Action execute;

        private Func<bool> canExecute;

        public MenuCommand()
        {

        }
        public MenuCommand(Action execute, Func<bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public void Execute(object parameter)
        {
            execute();
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute();
        }

        private void RaiseCanExecuteChanged()
        {
            // PresentationCore assembly
            // should this be here?
            CommandManager.InvalidateRequerySuggested();
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }
    }
}