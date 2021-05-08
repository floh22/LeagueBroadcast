using LeagueBroadcast.ChampSelect.Data.LCU;

namespace LeagueBroadcast.ChampSelect.StateInfo
{
    public class CurrentState
    {
        public bool isChampSelectActive;
        public Session session;

        public CurrentState(bool IsChampSelectActive, Session Session)
        {
            this.isChampSelectActive = IsChampSelectActive;
            this.session = Session;
        }
    }
}
