using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static LeagueBroadcast.Common.Data.Provider.DataDragon;
using LeagueBroadcast.Common.Controllers;
using LeagueBroadcast.MVVM.Core;
using LeagueBroadcast.MVVM.ViewModel;
using Newtonsoft.Json;
using LeagueBroadcast.Common.Data.DTO;
using LeagueBroadcast.Common.Data.RIOT;
using System.Threading;

namespace LeagueBroadcast.Common.Data.Provider
{
    class DataDragon : ObservableObject
    {
        public static readonly string currentDir = Directory.GetCurrentDirectory();
        private static DataDragon _instance;
        public static DataDragon Instance => GetInstance();

        public static GameVersion version;


        public List<int> FullIDs;

        public static EventHandler FinishLoading, StartLoading;

        public static EventHandler FileDownloadComplete;
        public static int ToDownload;

        public static void IncrementToDownload()
        {
            Interlocked.Increment(ref ToDownload);
        }

        public static void IncrementToDownload(int val)
        {
            Interlocked.Add(ref ToDownload, val);
        }


        private StartupViewModel _startupContext = (StartupViewModel) BroadcastController.Instance.Startup.DataContext;
        private int maxTasks = 0;
        private int progress = 0;

        public struct GameVersion
        {
            public string Version;
            public string Champion;
            public string Item;
            public string Summoner;

            public string CDN;

            public string GetVersionCDN()
            {
                return $"{CDN}/{Champion}";
            }
        }

        private static DataDragon GetInstance()
        {
            if (_instance == null)
            {
                _instance = new DataDragon();
            }

            return _instance;
        }

        private DataDragon()
        {
            
            Log.Info("DataDragon Provider Init");
            _startupContext.Status = "DataDragon Init";
            version = new GameVersion
            {
                Version = ConfigController.Component.DataDragon.Patch,
                CDN = ConfigController.Component.DataDragon.CDN

            };

            FullIDs = new List<int>();

            StartLoading?.Invoke(this, EventArgs.Empty);
            if (version.Version == "latest")
            {
                Log.Info("Getting latest versions from dataDragon");
                new Task(() => {
                    InitLatest();
                }).Start();
            }
            else
            {
                Log.Info($"Using version from configuration: {version.Version}");
                version.Champion = version.Version;
                version.Item = version.Version;
                version.Summoner = version.Version;
                new Task(() => { Init(); }).Start();
            }
        }

        private async void InitLatest()
        {
            _startupContext.Status = "Retrieving latest patch info";
            Log.Info("Retrieving latest patch info");
            dynamic riotVersion = JsonConvert.DeserializeObject<dynamic>(await DataDragonUtils.GetAsync($"https://ddragon.leagueoflegends.com/realms/{ConfigController.Component.DataDragon.Region}.json"));
            version.CDN = riotVersion.cdn;
            version.Champion = riotVersion.n.champion;
            version.Item = riotVersion.n.item;
            version.Summoner = riotVersion.n.summoner;

            var oldPatch = ConfigController.PickBan.frontend.patch;
            ConfigController.PickBan.frontend.patch = version.Champion;
            if (oldPatch != version.Champion)
            {
                Log.Info($"New patch {version.Champion} detected");
                ConfigController.UpdateConfigFile(ConfigController.PickBan);
            }

            Init();
        }

        private async void Init()
        {
            Log.Info($"Champion: {version.Champion}, Item: {version.Item}, CDN: {version.CDN}");

            Champion.Champions = new List<Champion>(JsonConvert.DeserializeObject<dynamic>(await DataDragonUtils.GetAsync($"{version.CDN}/{version.Champion}/data/{ConfigController.Component.DataDragon.Locale}/champion.json")).data.ToObject<Dictionary<string, Champion>>().Values);
            Log.Info($"Loaded {Champion.Champions.Count} champions");

            SummonerSpell.SummonerSpells = new List<SummonerSpell>(JsonConvert.DeserializeObject<dynamic>(await DataDragonUtils.GetAsync($"{version.CDN}/{version.Item}/data/{ConfigController.Component.DataDragon.Locale}/summoner.json")).data.ToObject<Dictionary<string, SummonerSpell>>().Values);
            Log.Info($"Loaded {SummonerSpell.SummonerSpells.Count} summoner spells");

            List<KeyValuePair<int, ItemData>> rawItemData = new List<KeyValuePair<int, ItemData>>(JsonConvert.DeserializeObject<dynamic>(await DataDragonUtils.GetAsync($"{version.CDN}/{version.Item}/data/{ConfigController.Component.DataDragon.Locale}/item.json")).data.ToObject<Dictionary<int, ItemData>>());
            Log.Info($"Detected {rawItemData.Count} items");

            rawItemData.ForEach(kvPair => {
                ItemData itemData = kvPair.Value;
                itemData.itemID = kvPair.Key;
                if (itemData.gold.total >= ConfigController.Component.DataDragon.MinimumGoldCost || itemData.specialRecipe != 0)
                {
                    ItemData.Items.Add(itemData);
                    FullIDs.Add(itemData.itemID);
                }
            });

            Log.Info($"Loaded {ItemData.Items.Count} full items");

            //Download all needed champion, item, and summoner spell data
            await CheckLocalCache();

            FinishLoading.Invoke(this, EventArgs.Empty);

        }

        public Champion GetChampionById(int champID)
        {
            var champData = Champion.Champions.Find(c => c.key == champID);
            if (champData != null)
            {
                DataDragonUtils.ExtendChampionLocal(champData, version);
            }

            return champData;
        }

        public SummonerSpell GetSummonerById(int summonerID)
        {
            var summonerData = SummonerSpell.SummonerSpells.Find(s => s.key == summonerID);
            if (summonerData != null)
            {
                DataDragonUtils.ExtendSummonerLocal(summonerData, version);
            }

            return summonerData;
        }

        public ItemData GetItemById(int itemID)
        {
            var itemData = ItemData.Items.Find(i => i.itemID == itemID);
            if (itemData != null)
            {
                DataDragonUtils.ExtendItemLocal(itemData, version);
            }

            return itemData;
        }

        public async Task<bool> CheckLocalCache()
        {
            _startupContext.Status = "Checking Cache";
            Log.Info("Checking Local Cache");

            int total = 0;
            FileDownloadComplete += (s, e) => {
                Interlocked.Increment(ref total);
            };

            var patch = version.Champion;
            var dlTasks = new List<Task>();

            string path = currentDir;
            string cache = path + "/Cache";
            string patchFolder = cache + "/" + ((string)patch);
            string champ = patchFolder + "/champion";
            string item = patchFolder + "/item";
            string spell = patchFolder + "/spell";

            maxTasks = Champion.Champions.Count * 4 + ItemData.Items.Count + SummonerSpell.SummonerSpells.Count;

            if (!Directory.Exists(cache))
            {
                Directory.CreateDirectory(cache);
            }

            if (!Directory.Exists(patchFolder))
            {
                //Delete old patch folders
                Log.Info("Current Patch cache not detected, removing old Patch data");
                _startupContext.Status = "Yeeting old patch onto Dominion map";

                List<string> dirs = new List<string>(Directory.EnumerateDirectories(cache).Where(d => !d.Contains("TeamIcons")));

                dirs.ForEach(dir => {
                    new DirectoryInfo(dir).Empty();
                    Directory.Delete(dir);
                    Log.Info($"Removed Patch Cache {dir}");
                });

                Directory.CreateDirectory(patchFolder);
            }
            else
            {
                Log.Info($"Cache {patchFolder} exists already");
                if(Directory.GetFiles(champ).Length < Champion.Champions.Count * 4)
                {
                    DownloadMissingChampionCache(champ);
                }
                if(Directory.GetFiles(item).Length < ItemData.Items.Count)
                {
                    DownloadMissingItemCache(item);
                }
                if(Directory.GetFiles(spell).Length < SummonerSpell.SummonerSpells.Count)
                {
                    DownloadMissingSummonerSpellCache(spell);
                } 
                return true;
            }

            if (!Directory.Exists(champ))
            {
                Directory.CreateDirectory(champ);
            }

            if (!Directory.Exists(item))
            {
                Directory.CreateDirectory(item);
            }

            if (!Directory.Exists(spell))
            {
                Directory.CreateDirectory(spell);
            }

            Log.Info("Starting download process. This could take a while");

            DownloadFullItemCache(item);
            DownloadFullChampionCache(champ);
            DownloadFullSummonerSpellCache(spell);

            _startupContext.UpdateDDragonProgress(0, dlTasks.Count);

            Log.Info($"Downloading {dlTasks.Count} assets from datadragon!");
            

            _startupContext.Status = $"Downloaded 0/{ToDownload} Images";
            
            while (total < ToDownload)
            {
                _startupContext.UpdateDDragonProgress(total, dlTasks.Count);
                _startupContext.Status = $"Downloaded {total}/{ToDownload} Images";
            }

            Log.Info("DataDragon download finished");
            _startupContext.Status = "DataDragon cache downloaded";
            return true;

        }

        private void DownloadFullChampionCache(string destUri)
        {
            Log.Info($"Downloading Champion Cache of {Champion.Champions.Count * 4} images");
            Champion.Champions.ForEach(findChampion => {
                DownloadChampionToCache(destUri, findChampion);
            });
        }

        private void DownloadMissingChampionCache(string destUri)
        {
            Champion.Champions.ForEach(champ =>
            {
                _startupContext.Status = $"Checking {champ} cache";
                DataDragonUtils.ExtendChampion(champ, version);
                if (!File.Exists($"{destUri}/{champ.id}_loading.png"))
                {
                    DataDragonUtils.DownloadFile(champ.loadingImg, $"{destUri}/{champ.id}_loading.png");
                    IncrementToDownload();
                }
                else
                {
                    _startupContext.UpdateDDragonProgress(progress++, maxTasks);
                }

                if (!File.Exists($"{destUri}/{champ.id}_splash.png"))
                {
                    DataDragonUtils.DownloadFile(champ.splashImg, $"{destUri}/{champ.id}_splash.png");
                    IncrementToDownload();
                }
                else
                {
                    _startupContext.UpdateDDragonProgress(progress++, maxTasks);
                }

                if (!File.Exists($"{destUri}/{champ.id}_centered_splash.png"))
                {
                    DataDragonUtils.DownloadFile(champ.splashCenteredImg, $"{destUri}/{champ.id}_centered_splash.png");
                    IncrementToDownload();
                }
                else
                {
                    _startupContext.UpdateDDragonProgress(progress++, maxTasks);
                }

                if (!File.Exists($"{destUri}/{champ.id}_square.png"))
                {
                    DataDragonUtils.DownloadFile(champ.squareImg, $"{destUri}/{champ.id}_square.png");
                    IncrementToDownload();
                }
                else
                {
                    _startupContext.UpdateDDragonProgress(progress++, maxTasks);
                }
            });
        }

        private void DownloadChampionToCache(string destUri, Champion champ)
        {
            DataDragonUtils.ExtendChampion(champ, version);
            DataDragonUtils.DownloadFile(champ.loadingImg, $"{destUri}/{champ.id}_loading.png");
            DataDragonUtils.DownloadFile(champ.splashImg, $"{destUri}/{champ.id}_splash.png");
            DataDragonUtils.DownloadFile(champ.splashCenteredImg, $"{destUri}/{champ.id}_centered_splash.png");
            DataDragonUtils.DownloadFile(champ.squareImg, $"{destUri}/{champ.id}_square.png");
            IncrementToDownload(4);
        }

        private void DownloadFullItemCache(string destUri)
        {
            Log.Info($"Downloading Item Cache of {ItemData.Items.Count} images");
            ItemData.Items.ForEach(findItem =>
            {
                DownloadItemToCache(destUri, findItem);
            });
        }

        private void DownloadMissingItemCache(string destUri)
        {
            ItemData.Items.ForEach(item => {
                _startupContext.Status = $"Checking {item} cache";
                if (!File.Exists($"{destUri}/{item.itemID}.png"))
                {
                    DownloadItemToCache(destUri, item);
                }
                else
                {
                    _startupContext.UpdateDDragonProgress(progress++, maxTasks);
                }
            });
        }

        private void DownloadItemToCache(string destUri, ItemData item)
        {
            DataDragonUtils.ExtendItem(item, version);
            DataDragonUtils.DownloadFile(item.sprite, $"{destUri}/{item.itemID}.png");
            IncrementToDownload();
        }
        
        private void DownloadFullSummonerSpellCache(string destUri)
        {
            Log.Info($"Downloading Summoner Spell Cache of {SummonerSpell.SummonerSpells.Count} images");
            SummonerSpell.SummonerSpells.ForEach(findSummoner => {
                DownloadSummonerSpellToCache(destUri, findSummoner);
            });
        }

        private void DownloadMissingSummonerSpellCache(string destUri)
        {
            SummonerSpell.SummonerSpells.ForEach(spell =>
            {
                _startupContext.Status = $"Checking {spell} cache";
                if (!File.Exists($"{destUri}/{spell.id}.png"))
                {
                    DownloadSummonerSpellToCache(destUri, spell);
                }
                else
                {
                    _startupContext.UpdateDDragonProgress(progress++, maxTasks);
                }
            });
        }

        private void DownloadSummonerSpellToCache(string destUri, SummonerSpell spell)
        {
            DataDragonUtils.ExtendSummoner(spell, version);
            DataDragonUtils.DownloadFile(spell.icon, $"{destUri}/{spell.id}.png");
            IncrementToDownload();
        }
    }

    static class DataDragonUtils
    {
        public static void Empty(this DirectoryInfo directory)
        {
            foreach (FileInfo file in directory.EnumerateFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo subDirectory in directory.EnumerateDirectories())
            {
                subDirectory.Delete(true);
            }
        }

        public static string CombinePaths(params string[] paths)
        {
            if (paths == null)
            {
                throw new ArgumentNullException("paths");
            }
            string final = "";
            paths.ToList().ForEach(part => final += $"/{part}");
            return final;
        }

        public static void DownloadFile(string uri, string path)
        {
            using var client = new WebClient();
            client.Headers.Add("accept", "*/*");

            client.DownloadDataCompleted += (sender, eventArgs) =>
            {
                try
                {
                    byte[] fileData = eventArgs.Result;
                    using FileStream fileStream = new FileStream(path, FileMode.Create);
                    fileStream.Write(fileData, 0, fileData.Length);
                    client.Dispose();
                    Log.Verbose($"{uri} downloaded");
                    FileDownloadComplete.Invoke(null, EventArgs.Empty);
                } catch(Exception e)
                {
                    Log.Warn($"Could not download {uri}\n{eventArgs.Error.Message}");
                    FileDownloadComplete.Invoke(null, EventArgs.Empty);
                }


            };
            try
            {
                Log.Verbose($"Downloading {uri}");
                client.DownloadDataAsync(new Uri(uri));
            } catch(Exception e)
            {
                Log.Warn($"Download error: {e.Message}");
            }
        }

        public static async Task<string> GetAsync(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Method = "GET";
            request.Timeout = 2000;
            try
            {
                Log.Verbose($"Downloading {uri}");
                using HttpWebResponse response = (HttpWebResponse) await request.GetResponseAsync();
                
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    Log.Warn($"Could not download {uri}: {response.StatusCode}");
                    return "";
                }
                using Stream stream = response.GetResponseStream();
                using StreamReader reader = new StreamReader(stream);
                Log.Verbose($"{uri} downloaded");
                return await reader.ReadToEndAsync();
            } catch(Exception e)
            {
                Log.Warn($"Could not download {uri}: {e.Message}");
                return "";
            }
            
        }

        public static void ExtendChampion(Champion champion, GameVersion version)
        {
            champion.splashImg = $"{version.CDN}/img/champion/splash/{champion.id}_0.jpg";
            champion.splashCenteredImg = $"https://cdn.communitydragon.org/{version.Version}/champion/{champion.id}/splash-art/centered";
            champion.squareImg = $"{version.GetVersionCDN()}/img/champion/{ champion.id}.png";
            champion.loadingImg = $"{version.CDN}/img/champion/loading/{champion.id}_0.jpg";
        }

        public static void ExtendChampionLocal(Champion champion, GameVersion version)
        {
            string championPath = CombinePaths("cache", version.Champion, "champion");
            champion.splashImg = $"{championPath}/{ champion.id}_splash.jpg";
            champion.splashCenteredImg = $"{championPath}/{champion.id}_centered_splash.png";
            champion.squareImg = $"{championPath}/{ champion.id}_square.png";
            champion.loadingImg = $"{championPath}/{ champion.id}_loading.png";
        }

        public static void ExtendSummoner(SummonerSpell summoner, GameVersion version)
        {
            summoner.icon = $"{version.GetVersionCDN()}/img/spell/{summoner.id}.png";
        }

        public static void ExtendSummonerLocal(SummonerSpell summoner, GameVersion version)
        {
            summoner.icon = CombinePaths("cache", version.Item, "spell", $"{summoner.id}.png");
        }

        public static void ExtendItem(ItemData item, GameVersion version)
        {
            item.sprite = $"{version.GetVersionCDN()}/img/item/{item.itemID}.png";
        }

        public static void ExtendItemLocal(ItemData item, GameVersion version)
        {
            item.sprite = CombinePaths("cache", version.Item, "item", item.itemID + ".png");
        }
    }
}