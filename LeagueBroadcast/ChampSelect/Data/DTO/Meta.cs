using LeagueBroadcast.Common.Data.Provider;

namespace LeagueBroadcast.ChampSelect.Data.DTO
{
    public class Meta
    {
        public string cdn => DataDragon.version.CDN;
        public Version version = new Version();
    }
}
