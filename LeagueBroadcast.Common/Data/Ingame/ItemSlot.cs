namespace LeagueBroadcast.Common.Data.Ingame
{
    public class ItemSlot
    {
        public bool IsEmpty { get; set; } = true;
        public int Slot { get; set; }
        public int ID { get; set; }
        public float Cost { get; set; }

        public ItemSlot(int slot = 0)
        {
            this.Slot = slot;
        }
    }
}
