using System.Windows;

namespace UpDownSingnalsServer.Views
{
    /// <summary>
    /// Interaction Loggeric for MessageWindow.xaml
    /// </summary>
    public partial class ErrorMessageWindow : Window
    {
        private bool _messageBoxSelection = false;

        public bool MessageBoxSelection
        {
            get { return this._messageBoxSelection; }
            set { this._messageBoxSelection = value; }
        }
        
        public ErrorMessageWindow()
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
