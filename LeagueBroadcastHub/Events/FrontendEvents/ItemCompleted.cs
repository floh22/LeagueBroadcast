
using LeagueBroadcastHub.Data.Containers;

namespace LeagueBroadcastHub.Events.FrontendEvents
{
    class ItemCompleted : LeagueEvent
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
