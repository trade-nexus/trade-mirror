using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Unity;
using UpDownSingnalsServer.ViewModels;

namespace UpDownSingnalsServer.Views
{
    /// <summary>
    /// Interaction logic for UpDownSignalsServerShell.xaml
    /// </summary>
    public partial class UpDownSignalsServerShell : Window
    {
        private ErrorMessageWindow _errorMessageWindow;
        private UpDownSignalsServerShellViewModel _signalsServerShellViewModel;

        public UpDownSignalsServerShell()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets MainWindowViewModel
        /// </summary>
        [Dependency]
        public UpDownSignalsServerShellViewModel UpDownSignalsServerShellViewModel
        {
            set
            {
                _signalsServerShellViewModel = value;
                this.DataContext = _signalsServerShellViewModel;
            }
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _errorMessageWindow = new ErrorMessageWindow { Owner = this };
            _errorMessageWindow.ShowDialog();
            if (_errorMessageWindow.MessageBoxSelection)
            {
                _signalsServerShellViewModel.FreeResources();
            }
            else
            {
                e.Cancel = true;
                _errorMessageWindow.Close();
            }
        }
    }
}
