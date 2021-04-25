using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcast.Ingame.Data.RIOT
{
    public class Rune
    {

        public string displayName;
        public int id;

        public Rune(int id, string displayName)
        {
            this.id = id;
            this.displayName = displayName;
        }
    }
}
