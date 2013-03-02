using System.Windows;

namespace UpDownSingnalsServer.Views
{
    /// <summary>
    /// Interaction logic for SubscriptionEndMessageWindow.xaml
    /// </summary>
    public partial class SubscriptionEndMessageWindow : Window
    {
        public SubscriptionEndMessageWindow()
        {
            InitializeComponent();
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
