using LeagueBroadcast.Common;
using LeagueBroadcast.Common.Data.DTO;
using LeagueBroadcast.Common.Utils;
using LeagueBroadcast.Farsight.Object;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LeagueBroadcast.Farsight
{
    public class FarsightController
    {
        public static List<string> Champions;

        public static Offsets GameOffsets = new();
        public static GameObject.Offsets ObjectOffsets = new();

        //Override to turn off memory reading at any point
        public static bool ShouldRun = true;

        public List<string> BlacklistedObjectNames = new() {
            "testcube",
            "testcuberender",
            "testcuberender10vision",
            "s5test_wardcorpse",
            "sru_camprespawnmarker",
            "sru_plantrespawnmarker",
            "preseason_turret_shield"
        };
        public List<int> BlacklistedObjects = new();
        public FarsightController()
        {
            if (!ShouldRun)
                return;
            Champions = CDragonChampion.All.Select(c => c.Alias).ToList();
            Log.Info($"Farsight loaded. Found {Champions.Count} Champ names");
        }

        public void Connect(Process p)
        {
            if (!ShouldRun)
                return;
            Memory.Initialize(p);

        }

        public Snapshot CreateSnapshot(double gameTime = 0)
        {


            Snapshot snap = new();
            if (!Memory.IsConnected || !ShouldRun)
            {
                return snap;
            }


            ReadObjects(snap);
            ClearMissing(snap);

            return snap;
        }


        private void ReadObjects(Snapshot snap)
        {
            int maxObjects = 500;
            IntPtr[] pointers = new IntPtr[maxObjects];

            byte[] buff = new byte[500];

            IntPtr objectManager = Memory.ReadMemory(Memory.m_baseAddress + GameOffsets.Manager, 8).ToIntPtr();

            Array.Copy(Memory.ReadMemory(objectManager, 100), 0, buff, 0, 100);

            Queue<IntPtr> toVisit = new();
            HashSet<IntPtr> visited = new();
            toVisit.Enqueue(buff.ToIntPtr(GameOffsets.MapRoot));

            int objNr = 0;
            int read = 0;
            IntPtr child1, child2, child3, node;

            while(read < maxObjects && toVisit.Count > 0)
            {
                node = toVisit.Dequeue();
                if(visited.Contains(node))
                    continue;

                read++;
                visited.Add(node);

                buff.Write(Memory.ReadMemory(node, 0x50));
                child1 = buff.ToIntPtr(0);
                child2 = buff.ToIntPtr(8);
                child3 = buff.ToIntPtr(16);

                toVisit.Enqueue(child1);
                toVisit.Enqueue(child2);
                toVisit.Enqueue(child3);

                uint netID = buff.ToUInt(GameOffsets.MapNodeNetId);

                if (netID - 0x40000000 > 0x100000)
                    continue;

                IntPtr addr = buff.ToIntPtr(GameOffsets.MapNodeObject);
                if (addr == IntPtr.Zero)
                    continue;

                pointers[objNr++] = addr;
            }

            for(int i = 0; i < objNr; i++)
            {
                int netID = Memory.ReadMemory(pointers[i] + ObjectOffsets.NetworkID, 4).ToInt();
                if (BlacklistedObjects.Contains(netID))
                    continue;

                GameObject obj;
                if(!snap.ObjectMap.ContainsKey(netID))
                {
                    obj = new();
                    obj.LoadFromMemory(pointers[i], ObjectOffsets.Level + 0x4);
                    snap.ObjectMap.Add(netID, obj);
                } else
                {
                    obj = snap.ObjectMap[netID];
                    obj.LoadFromMemory(pointers[i], ObjectOffsets.Level + 0x4);

                    if (netID != obj.NetworkID)
                        snap.ObjectMap[obj.NetworkID] = obj;
                }

                if (obj.NetworkID == 0)
                {
                    continue;
                }

                if(obj.DisplayName.StartsWith("Dragon_Indicator_"))
                {
                    //I hate myself for the naming convention but lets keep it consistent at least
                    snap.NextDragonType = obj.DisplayName.Replace(".troy", "").Remove(0, 17).Replace("Air", "Cloud", StringComparison.OrdinalIgnoreCase).Replace("Earth", "Mountain", StringComparison.OrdinalIgnoreCase).Replace("Water", "Ocean", StringComparison.OrdinalIgnoreCase);
                    continue;
                }

                snap.IndexToNetID[obj.ID] = obj.NetworkID;
                snap.UpdatedThisFrame.Add(obj.NetworkID);
                if (obj.Name.Length < 2 || BlacklistedObjectNames.Any(s => s.Equals(obj.Name, StringComparison.OrdinalIgnoreCase)))
                    BlacklistedObjects.Add(obj.NetworkID);

                if (obj.IsChampion())
                {
                    snap.Champions.Add(obj);
                    continue;
                }

                if (obj.Name.Contains("Turret", StringComparison.OrdinalIgnoreCase))
                {
                    snap.Turrets.Add(obj);
                    continue;
                }


                if (obj.Name.Contains("Dragon"))
                {
                    snap.Dragon = obj;
                    continue;
                }
                if(obj.Name.Equals("SRU_Baron"))
                {
                    snap.Baron = obj;
                    continue;
                }
                if(obj.Name.Equals("SRU_RiftHerald"))
                {
                    snap.Herald = obj;
                    continue;
                }
            }
        }

        private void ClearMissing(Snapshot snap)
        {
            foreach (var s in snap.ObjectMap.Keys.Where(key => !snap.UpdatedThisFrame.Contains(key)).ToList() )
            {
                snap.ObjectMap.Remove(s);
            }
        }

        public class Offsets
        {
            [JsonConverter(typeof(HexStringJsonConverter))]
            public int Manager;

            [JsonConverter(typeof(HexStringJsonConverter))]
            public int MapCount;

            [JsonConverter(typeof(HexStringJsonConverter))]
            public int MapRoot;

            [JsonConverter(typeof(HexStringJsonConverter))]
            public int MapNodeNetId;

            [JsonConverter(typeof(HexStringJsonConverter))]
            public int MapNodeObject;
        }
    }
}
