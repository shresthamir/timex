using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
namespace HRM.Library.Helpers
{
    /// <summary>
    /// Taken from http://msdn.microsoft.com/en-us/magazine/dd419663.aspx#id0090030
    /// </summary>
    
    public class RelayCommand : ICommand
    {
        #region Fields


       // readonly delegate object _DExecute();
        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;
        private RelayCommand DeleteCommand;

        #endregion // Fields

        #region Constructors

        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
            
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        public RelayCommand(RelayCommand DeleteCommand)
        {
            // TODO: Complete member initialization
            this.DeleteCommand = DeleteCommand;
        }
        #endregion // Constructors

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        

        #endregion // ICommand Members
    }

}
