using System.Windows;
using UpDownSingnalsClientTerminal.ViewModels;

namespace UpDownSingnalsClientTerminal.Views
{
    /// <summary>
    /// Interaction logic for ClientTerminal.xaml
    /// </summary>
    public partial class ClientTerminal : Window
    {
        private ApplicationViewModel _applicationViewModel;

        public ClientTerminal(ApplicationViewModel applicationViewModel)
        {
            InitializeComponent();
            _applicationViewModel = applicationViewModel;
            this.DataContext = applicationViewModel;
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //ToDo: Complete this
        }

        private void TradeSizeTypeSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _applicationViewModel.TradeSizeTypeSelectionChanged();
        }
    }
}
