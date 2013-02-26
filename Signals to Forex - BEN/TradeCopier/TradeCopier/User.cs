using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TradeCopier
{
    public class User : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int _id;
        public int Id
        {
            get { return _id; }
            set { if (value != _id) { _id = value; OnPropertyChanged("Id"); } }
        }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { { if (value != _userName) { _userName = value; OnPropertyChanged("UserName"); } } }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { { if (value != _password) { _password = value; OnPropertyChanged("Password"); } } }
        }

        private DateTime _created;
        public DateTime Created
        {
            get { return _created; }
            set { { if (value != _created) { _created = value; OnPropertyChanged("Created"); } } }
        }

        private bool _status;
        public bool Status
        {
            get { return _status; }
            set { { if (value != _status) { _status = value; OnPropertyChanged("Status"); } } }
        }

        private string _email;
        public string Email
        {
            get { return _email; }
            set { { if (value != _email) { _email = value; OnPropertyChanged("Email"); } } }
        }
        private void OnPropertyChanged(String stringName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(stringName));
            }
        }

    }
}
