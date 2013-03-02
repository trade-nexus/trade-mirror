using System.Windows;
using Microsoft.Practices.Unity;
using UpDownSingnalsClientTerminal.ViewModels;

namespace UpDownSingnalsClientTerminal.Views
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private ApplicationViewModel _applicationViewModel;

        public LoginWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets LoginWindowViewModel
        /// </summary>
        [Dependency]
        public ApplicationViewModel ApplicationViewModel
        {
            set
            {
                _applicationViewModel = value;
                this.DataContext = _applicationViewModel;
            }
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _applicationViewModel.FreeResources();
        }
    }
}
