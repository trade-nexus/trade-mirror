using System;

namespace AutoFXProfitsServer
{
    public class User : IComparable
    {
        #region Private Members

        private int _id;
        private string _email;
        private string _role;
        private bool _status;
        private int _accountNumber;
        private string _keyString;
        private DateTime _created;
        private DateTime _modified;
        private bool _sendNotifications;
        private string _alternativeEmail;

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

        public string Status
        {
            get
            {
                if (this._status == true)
                {
                    return "Active";
                }
                else
                {
                    return "Revoked";
                }
            }
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

        public string SendNotifications
        {
            get
            {
                if (this._sendNotifications == true)
                {
                    return "Yes";
                }
                else
                {
                    return "No";
                }
            }
        }

        public string AlternativeEmail
        {
            get { return this._alternativeEmail; }
            set { this._alternativeEmail = value; }
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
            this._sendNotifications = false;
            this._alternativeEmail = "alternative_email@default.com";
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
            this._sendNotifications = false;
            this._alternativeEmail = "alternative_email@default.com";
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
            this._email = email.ToLower();
            this._id = id;
            this._keyString = keyString;
            this._modified = modified;
            this._role = role;
            this._status = status;
            this._sendNotifications = false;
            this._alternativeEmail = "alternative_email@default.com";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="email"></param>
        /// <param name="role"></param>
        /// <param name="status"></param>
        /// <param name="accountNumber"></param>
        /// <param name="keyString"></param>
        /// <param name="created"></param>
        /// <param name="modified"></param>
        /// <param name="sendNotifications"></param>
        /// <param name="alternativeEmail"></param>
        public User(int id, string email, string role, bool status, int accountNumber, string keyString, DateTime created, DateTime modified, bool sendNotifications, string alternativeEmail)
        {
            this._accountNumber = accountNumber;
            this._created = created;
            this._email = email.ToLower();
            this._id = id;
            this._keyString = keyString;
            this._modified = modified;
            this._role = role;
            this._status = status;
            this._sendNotifications = sendNotifications;
            this._alternativeEmail = alternativeEmail;
        }

        #endregion

        public override string ToString()
        {
            return "Account Number = " + AccountNumber + " | Date Created = " + Created + " | Email ID = " + Email +
                   " | ID = " + ID + " | Key String = " + KeyString + " | Modified Date = " + Modified + " | Role = " +
                   Role + " | Status = " + Status + " | Send Notifications = " + SendNotifications +
                   " | Alternative Email = " + AlternativeEmail;
        }

        public int CompareTo(object obj)
        {
            User user2 = (User) obj;
            if (user2.AccountNumber == this.AccountNumber)
            {
                if (this.KeyString == user2.KeyString)
                {
                    if (this.Status == "Active")
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
