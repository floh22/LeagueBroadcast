using LeagueBroadcast.Common;
using LeagueBroadcast.Common.Utils;
using Newtonsoft.Json;
using System;
using System.Numerics;
using System.Text;

namespace LeagueBroadcast.Farsight.Object
{
    public class GameObject
    {
        private byte isChampion = byte.MaxValue;

        public const int buffDeepSize = 0x1000;

        public short ID;
        public int NetworkID;
        public short Team;
        public Vector3 Position;
        public string Name;
        public string DisplayName;
        public float Mana;
        public float MaxMana;
        public float Health;
        public float MaxHealth;
        public float CurrentGold;
        public float GoldTotal;
        public float EXP;
        public int Level;

        public void LoadFromMemory(IntPtr baseAdr, int buffSize = 0x4100)
        {
            //TODO Make VirtualQueryEx functional, currently always returns 0. If buff size every becomes a problem again, actually fix this
            if (buffSize == 0x0)
                buffSize = Memory.GetChampionObjectSize(baseAdr);

            var mem = Memory.ReadMemory(baseAdr, buffSize);

            ID = mem.ToShort(FarsightController.ObjectOffsets.ID);
            Team = mem.ToShort(FarsightController.ObjectOffsets.Team);
            Position = new Vector3(
                mem.ToFloat(FarsightController.ObjectOffsets.Pos),
                mem.ToFloat(FarsightController.ObjectOffsets.Pos + 4),
                mem.ToFloat(FarsightController.ObjectOffsets.Pos + 8)
            );
            Health = BitConverter.ToSingle(mem, FarsightController.ObjectOffsets.Health);
            MaxHealth = mem.ToFloat(FarsightController.ObjectOffsets.MaxHealth);
            Mana = mem.ToFloat(FarsightController.ObjectOffsets.Mana);
            MaxMana = mem.ToFloat(FarsightController.ObjectOffsets.MaxMana);
            NetworkID = mem.ToInt(FarsightController.ObjectOffsets.NetworkID);

            //Name = Memory.ReadMemory(Memory.ReadMemory(baseAdr + FarsightController.ObjectOffsets.Name, 8).ToIntPtr(), 50).DecodeAscii();
            
            int nameLength = mem.ToInt(FarsightController.ObjectOffsets.NameLength);
            if (nameLength <= 0 || nameLength > 100)
            {
                Name = "";
            }
            else if (nameLength < 16)
            {
                Name = Encoding.UTF8.GetString(mem.SubArray(FarsightController.ObjectOffsets.Name, nameLength));
            }
            else if (nameLength > 0)
            {
                Name = Encoding.UTF8.GetString(Memory.ReadMemory(mem.ToIntPtr(FarsightController.ObjectOffsets.Name), nameLength));
            }

            int displayNameLength = mem.ToInt(FarsightController.ObjectOffsets.DisplayNameLength);
            if (displayNameLength < 16)
            {
                DisplayName = Encoding.UTF8.GetString(mem.SubArray(FarsightController.ObjectOffsets.DisplayName, displayNameLength));
            }
            else
            {
                DisplayName = Encoding.UTF8.GetString(Memory.ReadMemory(mem.ToIntPtr(FarsightController.ObjectOffsets.DisplayName), displayNameLength));
            }

            if (IsChampion())
            {
                LoadChampFromMemory(mem);
            }
        }

        public void LoadChampFromMemory(byte[] source)
        {
            CurrentGold = source.ToFloat(FarsightController.ObjectOffsets.CurrentGold);
            GoldTotal = source.ToFloat(FarsightController.ObjectOffsets.GoldTotal);
            EXP = source.ToFloat(FarsightController.ObjectOffsets.EXP);
            Level = source.ToInt(FarsightController.ObjectOffsets.Level);
            Log.Verbose($"{Name} Gold: {CurrentGold}/{GoldTotal}, Exp:{EXP}/{Level}");
        }

        private byte LoadIsChampion()
        {
            for (int i = 0; i < FarsightController.Champions.Count; i++)
            {
                if (FarsightController.Champions[i].Equals(Name, StringComparison.OrdinalIgnoreCase))
                {
                    return 1;
                }
            }
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


        public class Offsets
        {
            [JsonConverter(typeof(HexStringJsonConverter))]
            public int ID;

            [JsonConverter(typeof(HexStringJsonConverter))]
            public int NetworkID;

            [JsonConverter(typeof(HexStringJsonConverter))]
            public int Team;

            [JsonConverter(typeof(HexStringJsonConverter))]
            public int DisplayName;

            [JsonConverter(typeof(HexStringJsonConverter))]
            public int DisplayNameLength;

            [JsonConverter(typeof(HexStringJsonConverter))]
            public int Pos;

            [JsonConverter(typeof(HexStringJsonConverter))]
            public int Mana;

            [JsonConverter(typeof(HexStringJsonConverter))]
            public int MaxMana;

            [JsonConverter(typeof(HexStringJsonConverter))]
            public int Health;

            [JsonConverter(typeof(HexStringJsonConverter))]
            public int MaxHealth;

            [JsonConverter(typeof(HexStringJsonConverter))]
            public int CurrentGold;

            [JsonConverter(typeof(HexStringJsonConverter))]
            public int GoldTotal;

            [JsonConverter(typeof(HexStringJsonConverter))]
            public int EXP;

            [JsonConverter(typeof(HexStringJsonConverter))]
            public int Level;

            [JsonConverter(typeof(HexStringJsonConverter))]
            public int Name;

            [JsonConverter(typeof(HexStringJsonConverter))]
            public int NameLength;
        }
    }

}
