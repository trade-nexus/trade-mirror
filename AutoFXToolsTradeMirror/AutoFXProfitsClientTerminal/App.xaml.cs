using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Unity;
using TraceSourceLogger;

namespace AutoFXProfitsClientTerminal
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly Type OType = typeof(App);

        private MainWindow _mainWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                base.OnStartup(e);

                AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

                Logger.Info("Exception handler hooked", OType.FullName, "OnStartup");

                IUnityContainer container = new UnityContainer();

                // Create main application window.
                _mainWindow = container.Resolve<MainWindow>();
                _mainWindow.Show();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "OnStartup");
            }
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            Logger.Error("Unhandled Application Level exception occured", OType.FullName, "CurrentDomainOnUnhandledException");
            _mainWindow.Close();
        }
    }
}
