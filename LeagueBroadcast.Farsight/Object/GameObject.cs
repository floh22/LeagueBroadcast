using LeagueBroadcast.Common;
using LeagueBroadcast.Common.Data.DTO;
using LeagueBroadcast.Common.Data.RIOT;
using LeagueBroadcast.Common.Utils;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Numerics;
using System.Text;

namespace LeagueBroadcast.Farsight.Object
{
	public class GameObject
	{
		private byte isChampion = byte.MaxValue;

		public const int buffSize = 0x4000;
		public const int buffDeepSize = 0x1000;

		public short ID;
		public int NetworkID;
		public short Team;
		public Vector3 Position;
		public string Name;
		public float Mana;
		public float MaxMana;
		public float Health;
		public float MaxHealth;
		public float CurrentGold;
		public float GoldTotal;
		public float EXP;
		public int Level;

		public void LoadFromMemory(int baseAdr, bool deepLoad = true)
        {
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
			
			if(deepLoad)
            {
				byte[] nameBuff = Memory.ReadMemory(Memory.ReadMemory(baseAdr + FarsightController.ObjectOffsets.Name, 4).ToInt(), 50);
				Name = nameBuff.DecodeAscii();

				if(IsChampion())
                {
					LoadChampFromMemory(mem, baseAdr, deepLoad);
                }
            }
        }

		public void LoadChampFromMemory(byte[] source, int baseAdr, bool deepLoad = true)
        {
            CurrentGold = source.ToFloat(FarsightController.ObjectOffsets.CurrentGold);
            GoldTotal = source.ToFloat(FarsightController.ObjectOffsets.GoldTotal);
            EXP = source.ToFloat(FarsightController.ObjectOffsets.EXP);
            Level = ChampionLevel.EXPToLevel(EXP);
        }

        private byte LoadIsChampion()
        {
			for (int i = 0; i < FarsightController.Champions.Count; i++)
            {
                if (FarsightController.Champions[i].Equals(Name, StringComparison.OrdinalIgnoreCase))
                    return (byte)1;
            }
			return (byte)0;

        }

        public bool IsChampion()
        {
            if (isChampion == byte.MaxValue)
                isChampion = LoadIsChampion();
			return isChampion != byte.MaxValue && isChampion != 0;
		}
		

		public class Offsets
        {
			[JsonConverter(typeof(HexStringJsonConverter))]
			public int ID = 0x20;

			[JsonConverter(typeof(HexStringJsonConverter))]
			public int NetworkID = 0xCC;

			[JsonConverter(typeof(HexStringJsonConverter))]
			public int Team = 0x4C;

			[JsonConverter(typeof(HexStringJsonConverter))]
			public int Pos = 0x1D8;

			[JsonConverter(typeof(HexStringJsonConverter))]
			public int Mana = 0x298;

			[JsonConverter(typeof(HexStringJsonConverter))]
			public int MaxMana = 0x2A8;

			[JsonConverter(typeof(HexStringJsonConverter))]
			public int Health = 0xD98;

			[JsonConverter(typeof(HexStringJsonConverter))]
			public int MaxHealth = 0xDA8;

			[JsonConverter(typeof(HexStringJsonConverter))]
			public int CurrentGold = 0x1B50;

			[JsonConverter(typeof(HexStringJsonConverter))]
			public int GoldTotal = 0x1B60;

			[JsonConverter(typeof(HexStringJsonConverter))]
			public int EXP = 0x37AC;

			[JsonConverter(typeof(HexStringJsonConverter))]
			public int Name = 0x304c;

			[JsonConverter(typeof(HexStringJsonConverter))]
			public int ItemList = 0x3714;

			[JsonConverter(typeof(HexStringJsonConverter))]
			public int SpellBook = 0x2BA0;


			[JsonConverter(typeof(HexStringJsonConverter))]
			public int ItemListItem = 0xC;

			[JsonConverter(typeof(HexStringJsonConverter))]
			public int ItemInfo = 0x20;

			[JsonConverter(typeof(HexStringJsonConverter))]
			public int ItemInfoId = 0x68;
		}
	}

}
