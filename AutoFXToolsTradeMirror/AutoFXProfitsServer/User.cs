using System;

namespace AutoFXProfitsServer
{
    public class User : IComparable
    {
        private readonly Type OType = typeof(User);

        #region Private Members

        private int _id;
        private string _email;
        private string _role;
        private bool _status;
        private int _accountNumber;
        private string _keyString;
        private DateTime _created;
        private DateTime _modified;

        #endregion

        #region PublicMembers

        public int ID
        {
            get { return this._id; }
            set { this._id = value; }
        }

        public string Email
        {
            get { return this._email; }
            set { this._email = value; }
        }

        public string Role
        {
            get { return this._role; }
            set { this._role = value; }
        }

        public bool Status
        {
            get { return this._status; }
            set { this._status = value; }
        }

        public int AccountNumber
        {
            get { return this._accountNumber; }
            set { this._accountNumber = value; }
        }

        public string KeyString
        {
            get { return this._keyString; }
            set { this._keyString = value; }
        }

        public DateTime Created
        {
            get { return this._created; }
            set { this._created = value; }
        }

        public DateTime Modified
        {
            get { return this._modified; }
            set { this._modified = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public User()
        {
            this._accountNumber = 0;
            this._created = DateTime.MinValue;
            this._email = "email@default.com";
            this._id = 0;
            this._keyString = "default";
            this._modified = DateTime.MaxValue;
            this._role = "user";
            this._status = false;
        }

        /// <summary>
        /// Basic Augmented Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="accountNumber"></param>
        /// <param name="keyString"></param>
        public User(int id, int accountNumber, string keyString)
        {
            this._accountNumber = accountNumber;
            this._created = DateTime.MinValue;
            this._email = "email@default.com";
            this._id = id;
            this._keyString = keyString;
            this._modified = DateTime.MaxValue;
            this._role = "user";
            this._status = true;
        }

        /// <summary>
        /// Complete Augmented Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="email"></param>
        /// <param name="role"></param>
        /// <param name="status"></param>
        /// <param name="accountNumber"></param>
        /// <param name="keyString"></param>
        /// <param name="created"></param>
        /// <param name="modified"></param>
        public User(int id, string email, string role, bool status, int accountNumber, string keyString, DateTime created, DateTime modified)
        {
            this._accountNumber = accountNumber;
            this._created = created;
            this._email = email;
            this._id = id;
            this._keyString = keyString;
            this._modified = modified;
            this._role = role;
            this._status = status;
        }

        #endregion

        public override string ToString()
        {
            return "Account Number = " + AccountNumber + " | Date Created = " + Created + " | Email ID = " + Email +
                   " | ID = " + ID + " | Key String = " + KeyString + " | Modified Date = " + Modified + " | Role = " +
                   Role + " | Status = " + Status;
        }

        public override bool Equals(object obj)
        {
            User user2 = (User)obj;
            if(this.AccountNumber == user2.AccountNumber)
            {
                if (this.KeyString == user2.KeyString)
                {
                    return true;
                }
                else
                {
                    //Logger.Debug("The Keystring provided is incorrect. Username = " + user2.AccountNumber + " | Password = " + user2.KeyString, OType.FullName, "Equals");
                    return false;
                }
            }
            else
            {
                //Logger.Debug("The UserName provided is incorrect. Username = " + user2.AccountNumber + " | Password = " + user2.KeyString, OType.FullName, "Equals");
                return false;
            }
        }

        public int CompareTo(object obj)
        {
            User user2 = (User) obj;
            if (user2.AccountNumber == this.AccountNumber)
            {
                if (this.KeyString == user2.KeyString)
                {
                    if (this.Status)
                    {
                        return 0;
                    }
                    else
                    {
                        return -1;
                    }
                    
                }
                else
                {
                    return -1;
                }
            }
            else if (user2.AccountNumber < this.AccountNumber)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }
}
