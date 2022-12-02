using LeagueBroadcast.Common.Data.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueBroadcast.Common.Data.Pregame.Events
{
    public class ChampionSelectStart : BroadcastEvent
    {
        public ChampionSelectStart() : base("champSelectStart") { }
    }
}
