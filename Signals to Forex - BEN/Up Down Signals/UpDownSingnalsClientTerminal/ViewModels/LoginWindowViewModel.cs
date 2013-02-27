using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace UpDownSingnalsClientTerminal.ViewModels
{
    public class LoginWindowViewModel : DependencyObject
    {
        private static readonly Type OType = typeof(LoginWindowViewModel);

        #region Username

        public static readonly DependencyProperty UsernameProperty =
            DependencyProperty.Register("Username", typeof (string), typeof (LoginWindowViewModel), new PropertyMetadata(default(string)));

        public string Username
        {
            get { return (string) GetValue(UsernameProperty); }
            set { SetValue(UsernameProperty, value); }
        }

        #endregion

        #region Password

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof (string), typeof (LoginWindowViewModel), new PropertyMetadata(default(string)));

        public string Password
        {
            get { return (string) GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        #endregion

        #region Status

        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(string), typeof(LoginWindowViewModel), new PropertyMetadata(default(string)));

        public string Status
        {
            get { return (string)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        #endregion

        #region Color

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(SolidColorBrush), typeof(LoginWindowViewModel), new PropertyMetadata(default(SolidColorBrush)));

        public SolidColorBrush Color
        {
            get { return (SolidColorBrush)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
        #endregion

        #region IsSavePasswordChecked

        public static readonly DependencyProperty IsSavePasswordCheckedProperty =
            DependencyProperty.Register("IsSavePasswordChecked", typeof(bool), typeof(LoginWindowViewModel), new PropertyMetadata(default(bool)));

        public bool IsSavePasswordChecked
        {
            get { return (bool)GetValue(IsSavePasswordCheckedProperty); }
            set { SetValue(IsSavePasswordCheckedProperty, value); }
        }

        #endregion

        public ICommand ConnectCommand { get; set; }
        public ICommand DisconnectCommand { get; set; }



        
    }
}
