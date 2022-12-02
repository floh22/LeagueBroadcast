using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace LeagueBroadcast.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ClientController? _clientController;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _clientController = ClientController.BuildClientControllerAsync().Result;
        }
    }
}
