using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace RemoteAdministrator
{
    public class ActiveUsersCheckedCommand : ICommand
    {
        private MainWindowViewModel _mainWindowViewModel;
        
        /// <summary>
        /// Argument Constructor
        /// </summary>
        /// <param name="mainWindowViewModel"></param>
        public ActiveUsersCheckedCommand(MainWindowViewModel mainWindowViewModel)
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
            _mainWindowViewModel.RadioButtonChecked("Active");
        }
        #endregion
    }
}
