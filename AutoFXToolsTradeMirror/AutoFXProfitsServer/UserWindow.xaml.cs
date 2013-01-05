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

        public UserWindow()
        {
            InitializeComponent();
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
    }
}
