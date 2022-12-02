using LeagueBroadcast.Common.Data.Farsight;
using LeagueBroadcast.Common.Data.CommunityDragon;
using LeagueBroadcast.Utils;
using LeagueBroadcast.Utils.Log;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using static LeagueBroadcast.Farsight.MemoryUtils;

namespace LeagueBroadcast.Farsight
{
    public static class FarsightDataProvider
    {
        public static HashSet<string> Champions { get; set; } = new();

        public static GlobalOffsets GameOffsets { get; set; } = new();
        public static GameObjectOffsets ObjectOffsets { get; set; } = new();

        //Override to turn off memory reading at any point
        public static bool ShouldRun { get; set; } = true;

        public static HashSet<string> BlacklistedObjectNames { get; set; } = new()
        {
            "testcube",
            "testcuberender",
            "testcuberender10vision",
            "s5test_wardcorpse",
            "sru_camprespawnmarker",
            "sru_plantrespawnmarker",
            "preseason_turret_shield"
        };
        public static List<int> BlacklistedObjects { get; set; } = new();

        private static int ObjectManagerLocation = 0;
        private static int HUDInstanceLocation = 0;
        private static int GameTimeLocation = 0;

        private static ListManagerTemplate? _heroList;
        private static ListManagerTemplate? _minionList;
        private static ListManagerTemplate? _turretList;


        //TODO: switch to using Lists instead of object manager for most reading. heavily speeds up read!
        static FarsightDataProvider()
        {
        }

        /// <summary>
        /// Returns the HeroList
        /// </summary>
        /// <returns><see cref="ListManagerTemplate" of HeroList/></returns>
        public static ListManagerTemplate GetHeroList()
        {
            _heroList ??= new ListManagerTemplate(GameOffsets.ObjectLists.Hero);
            return _heroList;
        }

        /// <summary>
        /// Returns the MinionList
        /// </summary>
        /// <returns><see cref="ListManagerTemplate" of MinionList/></returns>
        public static ListManagerTemplate GetMinionList()
        {
            _minionList ??= new ListManagerTemplate(GameOffsets.ObjectLists.Minion);
            return _minionList;
        }

        /// <summary>
        /// Returns the TurretList
        /// </summary>
        /// <returns><see cref="ListManagerTemplate" of TurretList/></returns>
        public static ListManagerTemplate GetTurretList()
        {
            _turretList ??= new ListManagerTemplate(GameOffsets.ObjectLists.Turret);
            return _turretList;
        }

        public static void Init()
        {
            Champions = Champion.All.Select(c => c.Alias).ToHashSet();
            $"Farsight loaded. Found {Champions.Count} Champ names".Info();
            return;
        }

        public static void Connect(Process p)
        {
            if (!ShouldRun)
                return;
            Initialize(p);

            do
            {
                ObjectManagerLocation = ReadMemory(m_baseAddress + GameOffsets.Manager, 4).ToInt();
                HUDInstanceLocation = ReadMemory(m_baseAddress + GameOffsets.HUDInstance, 4).ToInt();
                GameTimeLocation = m_baseAddress + GameOffsets.GameTime;
                if (ObjectManagerLocation == 0 || HUDInstanceLocation == 0 || GameTimeLocation == 0)
                    Task.Delay(100).Wait();
            } while (!p.HasExited && (ObjectManagerLocation == 0 || HUDInstanceLocation == 0 || GameTimeLocation == 0));

        }

        public static Snapshot CreateSnapshot(bool readItems)
        {
            Snapshot snap = new();
            if (!IsConnected || !ShouldRun)
            {
                "Could not create memory snapshot".Warn();
                return snap;
            }

            ReadObjects(snap, readItems);
            ClearMissing(snap);
            snap.GameTimeInSeconds = GetGameTimeInSeconds();
            return snap;
        }

        public static float GetGameTimeInMilliseconds()
        {
            return GetGameTimeInSeconds() * 1000;
        }

        public static float GetGameTimeInSeconds()
        {
            return ReadMemory<float>(GameTimeLocation);
        }

        public static GameObject GetGameObject(int networkID, bool readItems = false)
        {
            DateTime StartTime = DateTime.Now;
            int maxObjects = 500;
            GameObject obj = new();
            byte[] buff = new byte[500];

            Array.Copy(ReadMemory(ObjectManagerLocation, 100), 0, buff, 0, 100);

            Queue<int> toVisit = new();
            HashSet<int> visited = new();
            toVisit.Enqueue(buff.ToInt(GameOffsets.MapRoot));

            int read = 0;
            int child1, child2, child3, node;

            while (read < maxObjects && toVisit.Count > 0)
            {
                node = toVisit.Dequeue();
                if (visited.Contains(node))
                {
                    continue;
                }
                read++;
                visited.Add(node);
                buff.Write(ReadMemory(node, 0x30));
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

                if (netID == networkID)
                {
                    obj.LoadFromMemoryFull(addr, ObjectOffsets.Experience + 0x4, readItems);
                    break;
                }
            }

            $"Found GameObject in {(DateTime.Now - StartTime).TotalMilliseconds}ms".Debug();
            return obj;
        }

        public static async Task<List<string>> GetPlayerTabs()
        {
            List<string> data = new();

            //Ignore localhost cert
            HttpClientHandler handler = new()
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return true;
                }
            };
            try
            {
                using HttpClient httpClient = new(handler);
                httpClient.Timeout = TimeSpan.FromSeconds(2);
                HttpResponseMessage response = await httpClient.GetAsync("https://127.0.0.1:2999/liveclientdata/playerlist");
                if (!response.IsSuccessStatusCode)
                {
                    //Tournament Realm Game. Use champ select data if available or request user input
                    return data;
                }
                var content = JsonSerializer.Deserialize<JsonElement>(await response.Content.ReadAsStringAsync());
                foreach (JsonElement playerData in content.EnumerateArray())
                {
                    data.Add(playerData.GetProperty("summonerName").GetString() ?? "");
                }
            }
            catch (TaskCanceledException)
            {
                $"PlayerList request timed out. Is this Tournament Realm?".Warn();
            }
            catch (WebException ex)
            {
                $"{ex.Message}".Error();
            }


            return data;
        }



        private static void ReadObjects(Snapshot snap, bool readItems = false)
        {
            int maxObjects = 500;
            int[] pointers = new int[maxObjects];
            byte[] buff = new byte[500];
            int loc = ObjectManagerLocation;

            Array.Copy(ReadMemory(loc, 100), 0, buff, 0, 100);

            Queue<int> toVisit = new();
            HashSet<int> visited = new();
            toVisit.Enqueue(buff.ToInt(GameOffsets.MapRoot));

            int objNr = 0;
            int read = 0;
            int child1, child2, child3, node;

            while (read < maxObjects && toVisit.Count > 0)
            {
                node = toVisit.Dequeue();
                if (visited.Contains(node))
                {
                    continue;
                }

                read++;
                visited.Add(node);

                buff.Write(ReadMemory(node, 0x30));
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

            for (int i = 0; i < objNr; i++)
            {
                int netID = ReadMemory(pointers[i] + ObjectOffsets.NetworkID, 4).ToInt();
                if (BlacklistedObjects.Contains(netID))
                    continue;

                GameObject obj;
                if (!snap.ObjectMap.ContainsKey(netID))
                {
                    obj = new();
                    obj.LoadFromMemoryFull(pointers[i], ObjectOffsets.ItemList + 0x4 * 7, readItems);
                    snap.ObjectMap.Add(netID, obj);
                }
                else
                {
                    obj = snap.ObjectMap[netID];
                    obj.LoadFromMemoryFull(pointers[i], ObjectOffsets.ItemList + 0x4 * 7, readItems);

                    if (netID != obj.NetworkID)
                        snap.ObjectMap[obj.NetworkID] = obj;
                }

                if (obj.NetworkID != 0)
                {
                    snap.IndexToNetID[obj.ID] = obj.NetworkID;
                    snap.UpdatedThisFrame.Add(obj.NetworkID);
                    if (obj.ChampionID.Length < 2 || BlacklistedObjectNames.Any(s => s.Equals(obj.ChampionID, StringComparison.OrdinalIgnoreCase)))
                        BlacklistedObjects.Add(obj.NetworkID);
                }

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

                if (obj.Name.StartsWith("Inhibitor", StringComparison.OrdinalIgnoreCase))
                {
                    snap.Inhibitors.Add(obj);
                    continue;
                }

                if (obj.ChampionID.StartsWith("SRU_Dragon", StringComparison.OrdinalIgnoreCase))
                {
                    snap.Dragon = obj;
                    continue;
                }
                if (obj.ChampionID.Equals("SRU_Baron", StringComparison.OrdinalIgnoreCase))
                {
                    snap.Baron = obj;
                    continue;
                }
                if (obj.ChampionID.Equals("SRU_RiftHerald", StringComparison.OrdinalIgnoreCase))
                {
                    snap.Herald = obj;
                    continue;
                }
            }
        }

        public static List<GameObject> GetGameObjectsByNetworkID(HashSet<int> networkIds)
        {
            int maxObjects = 500;
            List<GameObject> champions = new();
            byte[] buff = new byte[500];

            Array.Copy(ReadMemory(ObjectManagerLocation, 100), 0, buff, 0, 100);

            Queue<int> toVisit = new();
            HashSet<int> visited = new();
            toVisit.Enqueue(buff.ToInt(GameOffsets.MapRoot));

            int read = 0;
            int child1, child2, child3, node;

            while (read < maxObjects && toVisit.Count > 0)
            {
                node = toVisit.Dequeue();
                if (visited.Contains(node))
                {
                    continue;
                }
                read++;
                visited.Add(node);
                buff.Write(ReadMemory(node, 0x30));
                child1 = buff.ToInt(0);
                child2 = buff.ToInt(4);
                child3 = buff.ToInt(8);

                toVisit.Enqueue(child1);
                toVisit.Enqueue(child2);
                toVisit.Enqueue(child3);

                int netID = buff.ToInt(GameOffsets.MapNodeNetId);

                if ((uint)netID - 0x40000000 > 0x100000)
                    continue;

                int addr = buff.ToInt(GameOffsets.MapNodeObject);
                if (addr == 0)
                    continue;

                if (networkIds.Contains(netID))
                {
                    GameObject toAdd = new();
                    toAdd.LoadFromMemoryFull(addr, ObjectOffsets.ItemList + 0x4 * 7);
                    champions.Add(toAdd);
                    if (champions.Count == 10)
                        break;
                }
            }
            return champions;
        }

        private static void ClearMissing(Snapshot snap)
        {
            foreach (var s in snap.ObjectMap.Keys.Where(key => !snap.UpdatedThisFrame.Contains(key)).ToList())
            {
                snap.ObjectMap.Remove(s);
            }
        }

    }
}


//world to screen
/*
 //Offsets
renderer = 0x316EE68;
renderer_width = 0x8;
renderer_height = 0xC;
view_proj_matrix = 0x31696A0;
 
//Var used
int width{};
int height{};
float	view_matrix[ 16 ]{};
float	proj_matrix[ 16 ]{};
float	v_proj_matrix[ 16 ]{};
DWORD renderer_address{};
 
 
//Read offsets from memory + apply matrix math
void load_from_mem( )
{
 
// renderer_adress can be read only once
	renderer_address = read<DWORD>( base_address + offset::renderer );
 
 
//Screen data
	height = read<int>( renderer_address + offset::renderer_height );
	width = read<int>( renderer_address + offset::renderer_width );
	
//Read matrix
	char buff[ 128 ]{};
	ReadProcessMemory( h_process, ( LPCVOID ) renderer_address, buff, 128, nullptr );
	ReadProcessMemory( h_process, ( LPCVOID ) (base_address + offset::view_proj_matrix ), buff, 128, nullptr );
 
	memcpy( view_matrix, &buff, 16 * sizeof( float ) );
	memcpy( proj_matrix, &buff[ 16 * sizeof( float ) ], 16 * sizeof( float ) );
        
//Matrix math
	for ( auto i = 0; i < 4; i++ )
	{
		for ( auto j = 0; j < 4; j++ )
		{
			auto sum = 0.f;
 
			for ( auto k = 0; k < 4; k++ )
			{
				sum = sum + view_matrix[ i * 4 + k ] * proj_matrix[ k * 4 + j ];
			}
 
			v_proj_matrix[ i * 4 + j ] = sum;
		}
	}
}
 
//World to screen logic
Vector2 w2s( const Vector3 pos )
{
//Be sure you called load_from_mem()
 
	const auto x = ( pos.x * v_proj_matrix[ 0 ] + pos.y * v_proj_matrix[ 4 ] + pos.z * v_proj_matrix[ 8 ] + v_proj_matrix[ 12 ] ) /
	                         ( pos.x * v_proj_matrix[ 3 ] + pos.y * v_proj_matrix[ 7 ] + pos.z * v_proj_matrix[ 11 ] + v_proj_matrix[ 15 ] );
 
	const auto y = ( pos.x * v_proj_matrix[ 1 ] + pos.y * v_proj_matrix[ 5 ] + pos.z * v_proj_matrix[ 9 ] + v_proj_matrix[ 13 ] ) /
		               ( pos.x * v_proj_matrix[ 3 ] + pos.y * v_proj_matrix[ 7 ] + pos.z * v_proj_matrix[ 11 ] + v_proj_matrix[ 15 ] );
 
	return
	{
			     ( width / 2 ) * x + ( x + ( width / 2 ) ) ,
	                -( ( height / 2 ) * y ) + ( y + ( height / 2 ) )
	};
}
*/