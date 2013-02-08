using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.IO;


namespace InstallerHelpProject
{
    [RunInstaller(true)]
    public partial class InstallerHelperClass : System.Configuration.Install.Installer
    {
        private const string EnvironementVariable = "USERPROFILE";
        public InstallerHelperClass()
        {
            InitializeComponent();

            string appDataLocation = System.Environment.GetEnvironmentVariable("APPDATA");
            Directory.CreateDirectory(appDataLocation + "\\AutoFX Profits Demo");
            Directory.CreateDirectory(appDataLocation + "\\AutoFX Profits Demo\\AutoFXProfitsClientTerminal");
            Directory.CreateDirectory(appDataLocation + "\\AutoFX Profits Demo\\logs");
        }


        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
        }


        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
            string targeted = this.Context.Parameters["targetdir"];

            DirFinder dr = new DirFinder();

            dr.GetInstalledSoftware2(targeted);    
        }

    }
}
