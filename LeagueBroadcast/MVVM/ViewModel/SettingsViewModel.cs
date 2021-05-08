using LeagueBroadcast.Common.Controllers;
using LeagueBroadcast.MVVM.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcast.MVVM.ViewModel
{
    class SettingsViewModel : ObservableObject
    {
        public bool AppUpdates
        {
            get { return ConfigController.Component.App.CheckForUpdates; }
            set { ConfigController.Component.App.CheckForUpdates = value; OnPropertyChanged(); }
        }

        public bool OffsetUpdate
        {
            get { return ConfigController.Component.App.CheckForOffsets; }
            set { ConfigController.Component.App.CheckForOffsets = value; OnPropertyChanged(); }
        }

    }
}
