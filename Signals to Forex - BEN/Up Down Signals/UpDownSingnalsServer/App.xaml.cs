using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Unity;
using UpDownSingnalsServer.Views;

namespace UpDownSingnalsServer
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

            if (DateTime.UtcNow < Convert.ToDateTime("3/3/2013"))
            {
                // Create main application window.
                UpDownSignalsServerShell upDownSignalsServerShell = container.Resolve<UpDownSignalsServerShell>();
                upDownSignalsServerShell.Show();
            }
            else
            {
                SubscriptionEndMessageWindow subscriptionEndMessageWindow = container.Resolve<SubscriptionEndMessageWindow>();
                subscriptionEndMessageWindow.Show();
            }
        }
    }
}
