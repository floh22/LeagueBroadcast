using LeagueBroadcast.Common.Controllers;
using LeagueBroadcast.Common.Data.Provider;
using LeagueBroadcast.MVVM.View;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;

namespace LeagueBroadcast
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static App Instance;
        protected override void OnStartup(StartupEventArgs e)
        {
            Instance = this;
            base.OnStartup(e);

            BroadcastController bController = BroadcastController.Instance;
        }
    }

    
}
