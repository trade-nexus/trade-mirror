using System;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Practices.Unity;

namespace AutoFXProfitsClientTerminal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _mainWindowViewModel;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets MainWindowViewModel
        /// </summary>
        [Dependency]
        public MainWindowViewModel MainWindowViewModel
        {
            set
            {
                _mainWindowViewModel = value;
                this.DataContext = _mainWindowViewModel;
            }
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this._mainWindowViewModel.FreeResources();
        }
    }
}
