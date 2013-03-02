﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using UpDownSingnalsClientTerminal.ViewModels;

namespace UpDownSingnalsClientTerminal.Command
{
    public class DisconnectCommand : ICommand
    {
        private ApplicationViewModel _applicationViewModel;
        
        /// <summary>
        /// Argument Constructor
        /// </summary>
        /// <param name="applicationViewModel"></param>
        public DisconnectCommand(ApplicationViewModel applicationViewModel)
        {
            this._applicationViewModel = applicationViewModel;
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
           _applicationViewModel.DisconnectFromServer();
        }
        #endregion
    }
}
