using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Practices.Unity;

namespace AutoFXProfitsServer
{
    /// <summary>
    /// Interaction logic for UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        private AutoFXToolsServerShellViewModel _autoFXToolsServerShellViewModel;

        public UserWindow(AutoFXToolsServerShellViewModel autoFXToolsServerShellViewModel)
        {
            InitializeComponent();
            _autoFXToolsServerShellViewModel = autoFXToolsServerShellViewModel;
            this.DataContext = _autoFXToolsServerShellViewModel;

            this.Activate();
            this.WindowStartupLocation= WindowStartupLocation.CenterOwner;
        }

        /// <summary>
        /// Sets MainWindowViewModel
        /// </summary>
        [Dependency]
        public AutoFXToolsServerShellViewModel AutoFXToolsServerShellViewModel
        {
            set
            {
                _autoFXToolsServerShellViewModel = value;
                this.DataContext = _autoFXToolsServerShellViewModel;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
