using LeagueBroadcast.Common.Data.RIOT;
using LeagueBroadcast.Common.Events;
using LeagueBroadcast.Ingame.Data.RIOT;

namespace LeagueBroadcast.Ingame.Events
{
    public class ItemCompleted : LeagueEvent
    {
        public int playerId;
        public ItemData itemData;

        public ItemCompleted(int playerId, ItemData itemData)
        {
            this.eventType = "ItemCompleted";
            this.playerId = playerId;
            this.itemData = itemData;
        }

    }
}
