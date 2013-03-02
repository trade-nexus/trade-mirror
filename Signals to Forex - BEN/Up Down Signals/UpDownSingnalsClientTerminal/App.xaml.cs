using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Unity;
using TraceSourceLogger;
using UpDownSingnalsClientTerminal.Views;

namespace UpDownSingnalsClientTerminal
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly Type OType = typeof(App);

        private LoginWindow _loginWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                base.OnStartup(e);

                AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

                Logger.Info("Exception handler hooked", OType.FullName, "OnStartup");

                IUnityContainer container = new UnityContainer();

                // Create login window.
                _loginWindow = container.Resolve<LoginWindow>();
                _loginWindow.Show();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, OType.FullName, "OnStartup");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="unhandledExceptionEventArgs"></param>
        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            Logger.Error("Unhandled Application Level exception occured", OType.FullName, "CurrentDomainOnUnhandledException");
            _loginWindow.Close();
        }
    }
}
