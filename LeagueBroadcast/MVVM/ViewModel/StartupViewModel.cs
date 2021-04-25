using LeagueBroadcast.Common.Controllers;
using LeagueBroadcast.MVVM.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcast.MVVM.ViewModel
{
    public class StartupViewModel : ObservableObject
    {
        public string AppVersion => $"v{BroadcastController.AppVersion}";

        private string _status;

        public string Status { get { return _status; } set { _status = value; OnPropertyChanged(); } }

        public StartupViewModel()
        {

        }
    }
}
