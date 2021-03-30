using LeagueBroadcastHub.Data.Game.Containers;

namespace LeagueBroadcastHub.Events.Game.FrontendEvents
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
