using System.Windows;
using Microsoft.Practices.Unity;

namespace RemoteAdministrator
{
    /// <summary>
    /// Interaction logic for UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        private MainWindowViewModel _mainWindowViewModel;

        public UserWindow(MainWindowViewModel mainWindowViewModel)
        {
            InitializeComponent();
            _mainWindowViewModel = mainWindowViewModel;
            this.DataContext = _mainWindowViewModel;

            this.Activate();
            this.WindowStartupLocation= WindowStartupLocation.CenterOwner;
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
