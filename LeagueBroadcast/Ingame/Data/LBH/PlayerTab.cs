using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcast.Ingame.Data.LBH
{
    public class PlayerTab
    {
        public string PlayerName;
        public string IconPath;
        public ValueBar Values;
        public string[] ExtraInfo;
    }

    public class ValueBar
    {
        public double MinValue;
        public double MaxValue;
        public double CurrentValue;
    }
}
