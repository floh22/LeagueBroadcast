using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LeagueBroadcast.Common.Events
{
    public static class FrontendWebServerEventHandler
    {
        public static event EventHandler? WebServerReady;

        public static void FireWebServerReady([CallerMemberName] string memberName = "")
        {
            WebServerReady?.Invoke(memberName, EventArgs.Empty);
        }
    }
}
