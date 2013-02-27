using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TradeCopier
{
    public class MyViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<User> _items;
        public ObservableCollection<User> Items
        {
            get { return _items; }
            set { { if (value != _items) { _items = value; OnPropertyChanged("Items"); } } }
        }
        public User CurrentUser { get; set; }
        private void OnPropertyChanged(string items)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(items));
            }
        }

        public MyViewModel()
        {
            //Initilzation of Traders List
            Items = new ObservableCollection<User>();
            Items.Add(new User { Id = 1, Created = DateTime.Now, Password = "abc12@134", Status = true, UserName = "Trader 1", Email = "abc@gmail.com" });
            Items.Add(new User { Id = 2, Created = DateTime.Now, Password = "add12-%@134", Status = true, UserName = "Trader 2", Email = "a77bc@gmail.com" });
            Items.Add(new User { Id = 3, Created = DateTime.Now, Password = "add12%@34", Status = false, UserName = "Trader 3", Email = "ab212c@gmail.com" });
            Items.Add(new User { Id = 4, Created = DateTime.Now, Password = "add12-%34", Status = true, UserName = "Trader 4", Email = "a2321bc@gmail.com" });


        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
