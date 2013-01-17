using System;
using System.Windows.Input;

namespace AutoFXProfitsServer.Commands
{
    public class SortByStatusCommand : ICommand
    {
        private AutoFXToolsServerShellViewModel _autoFXToolsServerShellViewModel;
        
        /// <summary>
        /// Argument Constructor
        /// </summary>
        /// <param name="autoFXToolsServerShellViewModel"></param>
        public SortByStatusCommand(AutoFXToolsServerShellViewModel autoFXToolsServerShellViewModel)
        {
            this._autoFXToolsServerShellViewModel = autoFXToolsServerShellViewModel;
        }

        #region ICommand Members
        /// <summary>
        /// Can execute the command. Should button be enabled or disabled.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Action when button is clicked
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            _autoFXToolsServerShellViewModel.SortBy("Status");
        }
        #endregion
    }
}
