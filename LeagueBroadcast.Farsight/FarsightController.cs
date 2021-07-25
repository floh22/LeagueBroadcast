using LeagueBroadcast.Common;
using LeagueBroadcast.Common.Data.DTO;
using LeagueBroadcast.Common.Utils;
using LeagueBroadcast.Farsight.Object;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

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
            Champions = Champion.Champions.Select(c => c.id).ToList();
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
            int[] pointers = new int[maxObjects];

            int objectManager = BitConverter.ToInt32(Memory.ReadMemory(Memory.m_baseAddress + GameOffsets.Manager, 4), 0);

            byte[] buff = new byte[500];

            Array.Copy(Memory.ReadMemory(objectManager, 100), 0, buff, 0, 100);

            Queue<int> toVisit = new();
            HashSet<int> visited = new();
            toVisit.Enqueue(buff.ToInt(GameOffsets.MapRoot));

            int objNr = 0;
            int read = 0;
            int child1, child2, child3, node;

            while(read < maxObjects && toVisit.Count > 0)
            {
                node = toVisit.Dequeue();
                if(visited.Contains(node))
                {
                    continue;
                }

                read++;
                visited.Add(node);

                buff.Write(Memory.ReadMemory(node, 0x30));
                child1 = buff.ToInt(0);
                child2 = buff.ToInt(4);
                child3 = buff.ToInt(8);

                toVisit.Enqueue(child1);
                toVisit.Enqueue(child2);
                toVisit.Enqueue(child3);

                uint netID = buff.ToUInt(GameOffsets.MapNodeNetId);

                if (netID - 0x40000000 > 0x100000)
                    continue;

                int addr = buff.ToInt(GameOffsets.MapNodeObject);
                if (addr == 0)
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
                    obj.LoadFromMemory(pointers[i], true, ObjectOffsets.EXP + 0x4);
                    snap.ObjectMap.Add(netID, obj);
                } else
                {
                    obj = snap.ObjectMap[netID];
                    obj.LoadFromMemory(pointers[i], true, ObjectOffsets.EXP + 0x4);

                    if (netID != obj.NetworkID)
                        snap.ObjectMap[obj.NetworkID] = obj;
                }

                if (obj.NetworkID != 0)
                {
                    snap.IndexToNetID[obj.ID] = obj.NetworkID;
                    snap.UpdatedThisFrame.Add(obj.NetworkID);
                    if (obj.Name.Length < 2 || BlacklistedObjectNames.Any(s => s.Equals(obj.Name, StringComparison.OrdinalIgnoreCase)))
                        BlacklistedObjects.Add(obj.NetworkID);
                }

                if (obj.IsChampion())
                {
                    snap.Champions.Add(obj);
                    continue;
                }
                    

                if(obj.Name.Contains("Dragon"))
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
