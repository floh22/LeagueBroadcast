using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcastHub.Data.Client.LCU
{
    public class Session
    {
        public List<Cell> myTeam = new List<Cell>();
        public List<Cell> theirTeam = new List<Cell>();
        public List<List<Action>> actions = new List<List<Action>>();
        public Timer timer = new Timer();
    }
}
