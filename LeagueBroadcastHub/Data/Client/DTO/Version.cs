using LeagueBroadcastHub.Data.Provider;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcastHub.Data.Client.DTO
{
    public class Version
    {
        public string champion => DataDragon.version.Champion;
        public string item => DataDragon.version.Item;
    }
}
