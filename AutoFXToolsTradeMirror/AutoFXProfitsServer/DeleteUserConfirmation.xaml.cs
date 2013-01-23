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

namespace AutoFXProfitsServer
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
