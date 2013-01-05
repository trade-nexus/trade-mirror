using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Unity;

namespace AutoFXProfitsServer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            IUnityContainer container = new UnityContainer();

            if(DateTime.UtcNow < Convert.ToDateTime("1/12/2013"))
            {
                // Create main application window.
                AutoFXToolsServerShell mainWindow = container.Resolve<AutoFXToolsServerShell>();
                mainWindow.Show();
            }
        }
    }
}
