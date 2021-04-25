using LeagueBroadcast.Ingame.Data.Provider;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcast.Common.Controllers
{
    class IngameController : ITickable
    {
        public IngameDataProvider LoLDataProvider;
        public void DoTick()
        {
            throw new NotImplementedException();
        }
    }
}
