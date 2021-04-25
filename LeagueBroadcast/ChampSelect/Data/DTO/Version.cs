using LeagueBroadcast.Common.Data.Provider;

namespace LeagueBroadcast.ChampSelect.Data.DTO
{
    public class Version
    {
        public string champion => DataDragon.version.Champion;
        public string item => DataDragon.version.Item;
    }
}
