namespace LeagueBroadcast.Common.Tickable
{
    public interface ITickable
    {
        public void DoTick();

        public bool IsTicking()
        {
            return TickController.IsTicking(this);
        }
    }
}
