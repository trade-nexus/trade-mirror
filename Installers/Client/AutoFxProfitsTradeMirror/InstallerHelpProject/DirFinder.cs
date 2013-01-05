using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;
//using TraceSourceLogger;

namespace InstallerHelpProject
{
    class DirFinder
    {
        private static readonly Type OType = typeof(DirFinder);

        private const string EnvironementVariable = "USERPROFILE";

        public DirFinder()
        {
            
        }

        /// <summary>
        /// Method GetInsatalledSoftware reteieves the paths of the MetaTrader softwares by opening up the key in the unintall folder and 
        /// reading Displayname value
        /// </summary>
        /// <summary>
        /// Method GetInstalledSoftware2 retrieves the installation path by reading the name of the key and then reteiving the installation path
        /// </summary>
        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public void GetInstalledSoftware2(string source)
        {
            try
            {
                //Declare the string to hold the list:
                string target = null;
                //The registry key's path
                string softwareKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
                //string softwareKey = @"Software\MetaQuotes Software";
                //Registry.CurrentUser.OpenSubKey(softwareKey);
                ////MessageBox.Show(softwareKey);

                using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(softwareKey))
                {
                    //Loop through the registry directory of installed softwares
                    foreach (string skName in rk.GetSubKeyNames())
                    {
                        ////MessageBox.Show("registry key of " + skName);
                        //Logger.Info("registry key of " + skName, OType.FullName, "GetInstalledSoftware2");
                        
                        using (RegistryKey sk = rk.OpenSubKey(skName))
                        {
                            try
                            {
                                string data = (string)sk.GetValue("DisplayName");
                                //string[] split = data.Split(new Char[] { ' ' });
                                if (data.Contains("MetaTrader") || data.Contains("MT"))
                                //if ((split[0] == "MetaTrader") || (split[1] == "MetaTrader") || (split[2] == "MetaTrader"))
                                {
                                    //MessageBox.Show("MetaTrader instance " + skName + " found");
                                    //Logger.Info("MetaTrader instance " + skName + " found", OType.FullName, "GetInstalledSoftware2");

                                    if (sk.GetValue("InstallLocation") == null)
                                    {
                                        target = sk.GetValue("DisplayName") + " - Install path not known\n";
                                        //MessageBox.Show("Not MetaTrader registry key");
                                        //Logger.Info("Not MetaTrader registry key", OType.FullName, "GetInstalledSoftware2");
                                    }
                                    else
                                    {
                                        target = (string)sk.GetValue("InstallLocation");
                                        //MessageBox.Show("Location where this MetaTrader is installed = " + target);
                                        //Logger.Info("Location where this MetaTrader is installed = " + target, OType.FullName, "GetInstalledSoftware2");
                                    }
                                    //MessageBox.Show(target);
                                    //Logger.Info("target = " + target, OType.FullName, "GetInstalledSoftware2");

                                    CopyFiles copier = new CopyFiles();
                                    copier.Run(source, target);

                                }
                                else
                                {
                                    //MessageBox.Show("Not the desired key");
                                    //Logger.Info("Not the desired key", OType.FullName, "GetInstalledSoftware2");
                                }
                            }
                            catch (Exception exception)
                            {
                                //MessageBox.Show(exception.ToString());
                                //Logger.Error(exception, OType.FullName, "GetInstalledSoftware2");
                            }
                        }
                    }
                }

                //C:\Users\Aurora\AppData\Roaming\MetaQuotes\Terminal\D0E8209F77C8CF37AD8BF550E51FF075\MQL5
                string temp = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                string appDataLoc = temp + "\\MetaQuotes\\Terminal\\D0E8209F77C8CF37AD8BF550E51FF075";
                //MessageBox.Show("App Data location = " + appDataLoc);
                //Logger.Info("App Data location = " + appDataLoc, OType.FullName, "GetInstalledSoftware2");

                CopyFiles copier2 = new CopyFiles();
                copier2.Run(source, appDataLoc);

                //  Delete(target);
            }

            catch (Exception exception)
            {
                //MessageBox.Show(exception.ToString());
                //Logger.Error(exception, OType.FullName, "GetInstalledSoftware2");
            }
        }

        /// <summary>
        /// Method delete to delete the file from the source directory after detecting it haas been copied to the target directory
        /// </summary>
        /// <param name="Software"></param>
        public void Delete(string target)
        {
            try
            {
                if (File.Exists(target))
                {
                    File.Delete(target);
                    //Logger.Debug("File deleted", OType.FullName, "Delete");
                    //MessageBox.Show("Deleted: " + target);
                }
            }

            catch (Exception exception)
            {
                //Logger.Error(exception, OType.FullName, "Delete");
                //MessageBox.Show(exception.ToString());
            }
        }
    }
}
