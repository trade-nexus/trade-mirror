using System;
using System.IO;
using System.ServiceModel;
using System.Timers;
using System.Windows.Forms;
using TraceSourceLogger;
using Color = System.Drawing.Color;
using Timer = System.Timers.Timer;

namespace AutoFXProfitsMACClientTerminal
{
    public partial class AutoFXProfitsClientTerminal : Form, ITradeMirrorCallback
    {
        private static readonly Type OType = typeof(AutoFXProfitsClientTerminal);

        private readonly InstanceContext _site;
        private TradeMirrorClient _client;

        private static Timer _heartbeatTimer;
        private const int AcceptedDelaySeconds = 60;

        public string AccountID
        {
            get { return AccountTextBox.Text; } 
            set { AccountTextBox.Text = value; }
        }

        public string KeyString
        {
            get { return KeyStringTextBox.Text; } 
            set { KeyStringTextBox.Text = value; }
        }

        public string Status
        {
            get { return StatusLabel.Text; }
            set { StatusLabel.Text = value; }
        }

        public bool IsSavePasswordChecked
        {
            get { return SaveKeyStringCheckBox.Checked; }
            set { SaveKeyStringCheckBox.Checked = value; }
        }

        public Color Color
        {
            get { return StatusLabel.ForeColor; }
            set { StatusLabel.ForeColor = value; }
        }

        public AutoFXProfitsClientTerminal()
        {
            try
            {
                InitializeComponent();

                // Create a client
                _site = new InstanceContext(null, this);
                _client = new TradeMirrorClient(_site);

                _heartbeatTimer = new Timer(AcceptedDelaySeconds * 1000);
                _heartbeatTimer.Elapsed += HeartbeatTimerElapsed;
                _heartbeatTimer.AutoReset = true;

                UpdateUI("Disconnected");

                if (!InitializeCredentials())
                {
                    SetAccountID();
                }
                else
                {
                    IsSavePasswordChecked = true;
                }

            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "AutoFXProfitsClientTerminal");
            }
        }

        /// <summary>
        /// Connect the client to the server
        /// </summary>
        public void ConnectToServer()
        {
            try
            {
                string suffixes = _client.Subscribe(AccountID, KeyString, Convert.ToInt32(AccountID));

                if (suffixes != "FAILED")
                {
                    Logger.Info("Subscribed", OType.FullName, "ConnectToServer");

                    UpdateUI("Connected");

                    SetSuffixes(suffixes);

                    _heartbeatTimer.Enabled = true;

                    if (IsSavePasswordChecked)
                    {
                        SaveAccountCredentials();
                    }
                    else
                    {
                        DeleteAccountCredentials();
                    }
                }
                else
                {
                    Logger.Info("Invalid Credentials", OType.FullName, "ConnectToServer");
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "ConnectToServer");
            }
        }

        /// <summary>
        /// Disconnect the client from the server
        /// </summary>
        public void DisconnectFromServer()
        {
            try
            {
                if (_client.Unsubscribe(AccountID, KeyString, Convert.ToInt32(AccountID)))
                {
                    Logger.Info("Unsubscribed", OType.FullName, "DisconnectFromServer");

                    UpdateUI("Disconnected");
                }
                else
                {
                    Logger.Info("Invalid Credentials", OType.FullName, "DisconnectFromServer");
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "DisconnectFromServer");
            }
        }

        /// <summary>
        /// Method called when a new signal arrives
        /// </summary>
        /// <param name="signalInformation"></param>
        public void NewSignal(string signalInformation)
        {
            try
            {
                if (signalInformation.Contains("___autofxtools trademirror___Alive___"))
                {
                    Logger.Debug("Heartbeat. = " + signalInformation, OType.FullName, "NewSignal");
                    ResetTimer();
                    return;
                }

                Logger.Debug("New Signal Received = " + signalInformation, OType.FullName, "NewSignal");
                PlaceOrder(signalInformation);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "NewSignal");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void HeartbeatTimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                DisconnectFromServer();
                Logger.Debug("Connection lost to server", OType.FullName, "HeartbeatTimerElapsed");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "HeartbeatTimerElapsed");
            }
        }

        /// <summary>
        /// Updates output on UI
        /// </summary>
        /// <param name="newStatus"></param>
        private void UpdateUI(string newStatus)
        {
            try
            {
                this.Status = newStatus;

                if (Status == "Connected")
                {
                    Color = Color.FromArgb(255, 0, 255, 0);
                }
                else if (Status == "Disconnected")
                {
                    Color = Color.FromArgb(255, 255, 0, 0);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "UpdateUI");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetAccountID()
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + @"\\accountinfo.txt";

                if (File.Exists(path))
                {
                    FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
                    StreamReader streamReader = new StreamReader(fs);

                    string tempString = string.Empty;

                    while ((tempString = streamReader.ReadLine()) != null)
                    {
                        if (tempString.Contains("accountNumber"))
                        {
                            string[] tempArray = tempString.Split(':');
                            AccountID = tempArray[1].Trim();
                        }
                        break;
                    }

                    streamReader.Close();
                    fs.Close();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "SetAccountID");
            }
        }

        /// <summary>
        /// Saves order into a text file
        /// </summary>
        public void PlaceOrder(string orderInfo)
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "\\orders.csv";

                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                using (File.Create(path))
                {
                }

                var fileStream = new FileStream(path, FileMode.Open, FileAccess.Write, FileShare.None);
                var streamWriter = new StreamWriter(fileStream);

                streamWriter.Write(orderInfo);
                Logger.Debug("Signal Information Written to file", OType.FullName, "PlaceOrder");

                streamWriter.Close();
                fileStream.Close();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "PlaceOrder");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SaveAccountCredentials()
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + @"\\trademirrorcredentials.txt";

                if (!File.Exists(path))
                {
                    FileStream fs = File.Create(path);
                    StreamWriter streamWriter = new StreamWriter(fs);

                    streamWriter.Write(AccountID + " : " + KeyString);
                    Logger.Debug("User Credentials saved. Account ID = " + AccountID + " | Keystring = " + KeyString, OType.FullName, "PlaceOrder");

                    streamWriter.Close();
                    fs.Close();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "SaveAccountCredentials");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void DeleteAccountCredentials()
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + @"\\trademirrorcredentials.txt";

                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                Logger.Debug("User Credentials Deleted" + KeyString, OType.FullName, "SaveAccountCredentials");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "SaveAccountCredentials");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool InitializeCredentials()
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + @"\\trademirrorcredentials.txt";

                if (File.Exists(path))
                {
                    FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
                    StreamReader streamReader = new StreamReader(fs);

                    string tempString = streamReader.ReadLine();

                    string[] tempArray = tempString.Split(':');

                    AccountID = tempArray[0].Trim();
                    KeyString = tempArray[1].Trim();

                    Logger.Debug("User Credentials Initialized. Account ID = " + AccountID + " | Keystring = " + KeyString, OType.FullName, "InitializeCredentials");

                    streamReader.Close();
                    fs.Close();

                    return true;
                }
                else
                {
                    Logger.Debug("User Credentials not available", OType.FullName, "InitializeCredentials");
                    return false;
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "InitializeCredentials");
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void FreeResources()
        {
            try
            {
                if (Status == "Connected")
                {
                    this.DisconnectFromServer();
                }
                if (_client != null)
                {
                    _client.Close();
                    _client.Abort();
                    _client = null;
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "FreeResources");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ResetTimer()
        {
            try
            {
                _heartbeatTimer.Stop();
                _heartbeatTimer.Start();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "ResetTimer");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="suffixes"></param>
        private void SetSuffixes(string suffixes)
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "\\suffixes.csv";

                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                using (File.Create(path))
                {
                }

                FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Write, FileShare.None);
                StreamWriter streamWriter = new StreamWriter(fileStream);

                streamWriter.Write(suffixes);
                Logger.Debug("System Suffixes = " + suffixes, OType.FullName, "SetSuffixes");
                streamWriter.Write(";");

                streamWriter.Close();
                fileStream.Close();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "SetSuffixes");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectButtonClick(object sender, EventArgs e)
        {
            ConnectToServer();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisconnectButtonClick(object sender, EventArgs e)
        {
            DisconnectFromServer();
        }

        private void AutoFXProfitsClientTerminalFormClosing(object sender, FormClosingEventArgs e)
        {
            FreeResources();
        }
    }
}
