using LeagueBroadcast.Common.Data.Ingame;
using LeagueBroadcast.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LeagueBroadcast.Farsight
{
    public class GameObject
    {
        private byte isChampion = byte.MaxValue;

        public const int buffDeepSize = 0x1000;

        public short ID;
        public int NetworkID;
        public short Team;
        public Vector3 Position;
        public string ChampionID = "";
        public float Mana;
        public float MaxMana;
        public float Health;
        public float MaxHealth;
        public float CurrentGold;
        public float GoldTotal;
        public float EXP;
        public int Level;
        public string Name = "";
        public ItemSlot[]? Items;


        public void LoadFromMemoryFast(int baseAdr, int buffSize = 0x3600)
        {
            byte[] mem = MemoryUtils.ReadMemory(baseAdr, buffSize);

            ID = mem.ToShort(FarsightDataProvider.ObjectOffsets.ID);
            Team = mem.ToShort(FarsightDataProvider.ObjectOffsets.Team);
            Position = new Vector3(
                mem.ToFloat(FarsightDataProvider.ObjectOffsets.Pos),
                mem.ToFloat(FarsightDataProvider.ObjectOffsets.Pos + 4),
                mem.ToFloat(FarsightDataProvider.ObjectOffsets.Pos + 8)
                );
            Health = BitConverter.ToSingle(mem, FarsightDataProvider.ObjectOffsets.Health);
            MaxHealth = mem.ToFloat(FarsightDataProvider.ObjectOffsets.HealthMax);
            NetworkID = mem.ToInt(FarsightDataProvider.ObjectOffsets.NetworkID);
        }

        public void LoadFromMemoryFull(int baseAdr, int buffSize = 0x3600, bool readItems = true)
        {
            byte[] mem = MemoryUtils.ReadMemory(baseAdr, buffSize);

            ID = mem.ToShort(FarsightDataProvider.ObjectOffsets.ID);
            Team = mem.ToShort(FarsightDataProvider.ObjectOffsets.Team);
            Position = new Vector3(
                mem.ToFloat(FarsightDataProvider.ObjectOffsets.Pos),
                mem.ToFloat(FarsightDataProvider.ObjectOffsets.Pos + 4),
                mem.ToFloat(FarsightDataProvider.ObjectOffsets.Pos + 8)
                );
            Health = BitConverter.ToSingle(mem, FarsightDataProvider.ObjectOffsets.Health);
            MaxHealth = mem.ToFloat(FarsightDataProvider.ObjectOffsets.HealthMax);
            Mana = mem.ToFloat(FarsightDataProvider.ObjectOffsets.Mana);
            MaxMana = mem.ToFloat(FarsightDataProvider.ObjectOffsets.ManaMax);
            NetworkID = mem.ToInt(FarsightDataProvider.ObjectOffsets.NetworkID);
            ChampionID = MemoryUtils.ReadMemory(mem.SubArray(FarsightDataProvider.ObjectOffsets.ChampionName, 4).ToInt(), 50).DecodeAscii();

            int nameLength = mem.ToInt(FarsightDataProvider.ObjectOffsets.PlayerNameLength);
            if (nameLength < 16)
            {
                Name = Encoding.UTF8.GetString(mem.SubArray(FarsightDataProvider.ObjectOffsets.PlayerNameLocation, nameLength));
            }
            else
            {
                Name = Encoding.UTF8.GetString(MemoryUtils.ReadMemory(mem.ToInt(FarsightDataProvider.ObjectOffsets.PlayerNameLocation), nameLength));
            }
       

            if (IsChampion())
            {
                CurrentGold = mem.ToFloat(FarsightDataProvider.ObjectOffsets.GoldCurrent);
                GoldTotal = mem.ToFloat(FarsightDataProvider.ObjectOffsets.GoldTotal);
                EXP = mem.ToFloat(FarsightDataProvider.ObjectOffsets.Experience);
                Level = ChampionLevel.EXPToLevel(EXP);


                if (!readItems)
                    return;

                Items = new ItemSlot[7];

                for(int i = 0; i < 7; i++)
                {
                    Items[i] = new ItemSlot(i) { IsEmpty = true, Slot = i };

                    int containerPtr = mem.ToInt(FarsightDataProvider.ObjectOffsets.ItemList + i * 4);

                    int itemPtr = MemoryUtils.ReadMemory<int>(containerPtr + FarsightDataProvider.ObjectOffsets.ItemListItem);

                    if (itemPtr == 0)
                        continue;

                    int itemInfoPtr = MemoryUtils.ReadMemory<int>(itemPtr + FarsightDataProvider.ObjectOffsets.ItemInfo);

                    if (itemInfoPtr == 0)
                        continue;

                    int id = MemoryUtils.ReadMemory<int>(itemInfoPtr + FarsightDataProvider.ObjectOffsets.ItemInfoId);
                    Items[i].IsEmpty = false;
                    Items[i].ID = id;
                }

      
                //Debug
                /*
                List<string> itemIds = Items.Where(i => i.IsEmpty == false).Select(i => Item.All.FirstOrDefault(Item => Item.ID == i.ID)).Select(Item => Item.Name).ToList();

                if (ChampionID == "Syndra")
                {
                    $"{String.Join(", ", itemIds)}".Debug();
                }
                */

                return;
            }
        }

        private byte LoadIsChampion()
        {
            if (FarsightDataProvider.Champions.Contains(ChampionID))
                return 1;
            return 0;

        }

        public bool IsChampion()
        {
            if (isChampion == byte.MaxValue)
            {
                isChampion = LoadIsChampion();
            }

            return isChampion is not byte.MaxValue and not 0;
        }

        
    }
}
