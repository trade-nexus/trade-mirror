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
                var copier = new CopyFiles();

                var files = new List<string>();
                foreach (var d in DriveInfo.GetDrives().Where(x => x.IsReady == true))
                {
                    files.AddRange(GetFiles(d.RootDirectory.FullName, "terminal.exe"));
                }
                //MessageBox.Show(files.Count + " MetaTrader Instances found");
                foreach (var file in files)
                {
                    string path = Path.GetDirectoryName(file);
                    copier.Run(source, path);
                }
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
        /// <param name="target"></param>
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

        public static IEnumerable<string> GetFiles(string root, string searchPattern)
        {
            Stack<string> pending = new Stack<string>();
            pending.Push(root);
            while (pending.Count != 0)
            {
                var path = pending.Pop();
                string[] next = null;
                try
                {
                    next = Directory.GetFiles(path, searchPattern);
                }
                catch { }
                if (next != null && next.Length != 0)
                    foreach (var file in next) yield return file;
                try
                {
                    next = Directory.GetDirectories(path);
                    foreach (var subdir in next) pending.Push(subdir);
                }
                catch { }
            }
        }
    }
}
