using LeagueBroadcast.Common.Controllers;
using LeagueBroadcast.Common.Events;
using LeagueBroadcast.Ingame.Data.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcast.Ingame.Events
{
    class IngameOverlay : OverlayConfig
    {
        private static IngameOverlay _instance;

        [JsonIgnore]
        public static IngameOverlay Instance => GetInstance();

        public IngameConfig config => ConfigController.Ingame;

        public IngameOverlay()
        {
            type = Http.FrontEndType.Ingame;
        }


        public static IngameOverlay GetInstance()
        {
            if (_instance != null)
                return _instance;

            _instance = new IngameOverlay();
            return _instance;
        }
    }
}
