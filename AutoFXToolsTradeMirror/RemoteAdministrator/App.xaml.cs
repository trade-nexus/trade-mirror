using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Unity;

namespace RemoteAdministrator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly Type OType = typeof(App);

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                base.OnStartup(e);

                IUnityContainer container = new UnityContainer();

                // Create main application window.
                MainWindow mainWindow = container.Resolve<MainWindow>();
                mainWindow.Show();
            }
            catch (Exception exception)
            {
                TraceSourceLogger.Logger.Error(exception, OType.FullName, "OnStartup");
            }
        }
    }
}
