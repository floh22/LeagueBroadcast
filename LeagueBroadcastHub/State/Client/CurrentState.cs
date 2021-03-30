using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcastHub.State.Client
{
    public class CurrentState
    {
        public bool isChampSelectActive;
        public Data.Client.LCU.Session session;

        public CurrentState(bool IsChampSelectActive, Data.Client.LCU.Session Session)
        {
            this.isChampSelectActive = IsChampSelectActive;
            this.session = Session;
        }
    }
}
