using System;
using System.Windows.Input;

namespace RemoteAdministrator
{
    public class SaveUserCommand : ICommand 
    {
        private MainWindowViewModel _mainWindowViewModel;
        
        /// <summary>
        /// Argument Constructor
        /// </summary>
        /// <param name="mainWindowViewModel"></param>
        public SaveUserCommand(MainWindowViewModel mainWindowViewModel)
        {
            this._mainWindowViewModel = mainWindowViewModel;
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
            this._mainWindowViewModel.SaveUser();
        }
        #endregion
    }
}
