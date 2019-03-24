using ColaApp.Client.ViewModels;
using CommonServiceLocator;
using Dna;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions;

namespace ColaApp.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {    

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Setup the main application 
            ApplicationSetup();

            // Show the login window
            Current.MainWindow = new LoginView();
            Current.MainWindow.Show();
        }

        private void ApplicationSetup()
        {            
            // Setup the Dna Framework
            Framework.Construct<DefaultFrameworkConstruction>()
                .AddFileLogger()
                .Build();

            // Monitor for server connection status
            MonitorServerStatus();
        }
  

        /// <summary>
        /// Monitors the server is up, running and reachable
        /// by periodically hitting it up
        /// </summary>
        private static void MonitorServerStatus()
        {
            // Create a new endpoint watcher
            var httpWatcher = new HttpEndpointChecker(
                // Checking fasetto.chat
                Dna.FrameworkDI.Configuration["ServerConfig:CheckInternetUrl"],
                // Every 1 seconds
                interval: 1000,
                // Pass in the DI logger
                logger: Framework.Provider.GetService<ILogger>(),
                // On change...
                stateChangedCallback: (result) =>
                {
                    // Update the view model property with the new result
                    ViewModelLocator.ApplicationViewModel.ServerReachable = result;
                    //DI.DI.ApplicationViewModelStatic.ServerReachable = result;
                });
        }
    }
}
