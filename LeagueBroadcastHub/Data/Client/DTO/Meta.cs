using LeagueBroadcastHub.Data.Provider;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcastHub.Data.Client.DTO
{
    public class Meta
    {
        public string cdn => DataDragon.version.CDN;
        public Version version = new Version();
    }
}
