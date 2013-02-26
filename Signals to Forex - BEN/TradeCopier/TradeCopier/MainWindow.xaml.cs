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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TradeCopier
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MyViewModel _vm ;
        public MainWindow()
        {
            InitializeComponent();
            _vm = new MyViewModel();
            this.DataContext = _vm;
        }

        private void Add_User_Click(object sender, RoutedEventArgs e)
        {
            Window addUser = new AddUser();
            addUser.Show();
        }

        private void Click_Browse(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = "CSV files (*.csv)|*.csv|XML files (*.xml)|*.xml";

            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();
        }

        private void EditUser(object sender, RoutedEventArgs e)
        {
            Window editUser = new EditUser();
            editUser.Show();
            editUser.DataContext = _vm.CurrentUser;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
