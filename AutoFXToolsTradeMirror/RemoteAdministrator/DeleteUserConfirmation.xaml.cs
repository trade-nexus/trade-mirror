using System.Windows;

namespace RemoteAdministrator
{
    /// <summary>
    /// Interaction logic for DeleteUserConfirmation.xaml
    /// </summary>
    public partial class DeleteUserConfirmation : Window
    {
        private bool _messageBoxSelection = false;

        public bool MessageBoxSelection
        {
            get { return this._messageBoxSelection; }
            set { this._messageBoxSelection = value; }
        }

        public DeleteUserConfirmation()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Called when YES button is Clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void YesButtonClicked(object sender, RoutedEventArgs e)
        {
            this._messageBoxSelection = true;
            this.Hide();
        }

        /// <summary>
        /// Called when NO button is Clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NoButtonClicked(object sender, RoutedEventArgs e)
        {
            this._messageBoxSelection = false;
            this.Hide();
        }
    }
}
