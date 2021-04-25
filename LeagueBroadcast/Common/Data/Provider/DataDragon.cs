using LeagueBroadcast.ChampSelect.Data.DTO;
using LeagueBroadcast.Ingame.Data.RIOT;
using LeagueBroadcast.OperatingSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using static LeagueBroadcast.Common.Data.Provider.DataDragon;
using LeagueBroadcast.Common.Controllers;
using LeagueBroadcast.MVVM.Core;

namespace LeagueBroadcast.Common.Data.Provider
{
    class DataDragon : ObservableObject
    {
        public static readonly string currentDir = Directory.GetCurrentDirectory();
        private static DataDragon _instance;
        public static DataDragon Instance => GetInstance();
        const string Realm = "euw";

        public static GameVersion version;

        List<Champion> Champions;
        List<SummonerSpell> SummonerSpells;
        List<ItemData> Items;

        public List<int> FullIDs;

        public static EventHandler FinishLoading, StartLoading;

        private string Status { set { BroadcastController.Instance.Startup.GETDataContext().Status = value; } }

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
                _instance = new();
            return _instance;
        }

        private DataDragon()
        {
            Status = "DataDragon Provider Init";
            Log.Info("DataDragon Provider Init");
            version = new GameVersion
            {
                Version = ConfigController.PickBan.contentPatch,
                CDN = ConfigController.PickBan.contentCdn

            };

            Champions = new List<Champion>();
            SummonerSpells = new List<SummonerSpell>();
            Items = new List<ItemData>();
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
            Status = "Retrieving latest patch info";
            Log.Info("Retrieving latest patch info");
            dynamic riotVersion = JsonSerializer.Deserialize<dynamic>(await DataDragonUtils.GetAsync($"https://ddragon.leagueoflegends.com/realms/{Realm}.json"));
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

            Champions = new List<Champion>(JsonSerializer.Deserialize<dynamic>(await DataDragonUtils.GetAsync($"{version.CDN}/{version.Champion}/data/en_US/champion.json")).data.ToObject<Dictionary<string, Champion>>().Values);
            Log.Info($"Loaded {Champions.Count} champions");

            SummonerSpells = new List<SummonerSpell>(JsonSerializer.Deserialize<dynamic>(await DataDragonUtils.GetAsync($"{version.CDN}/{version.Item}/data/en_US/summoner.json")).data.ToObject<Dictionary<string, SummonerSpell>>().Values);
            Log.Info($"Loaded {SummonerSpells.Count} summoner spells");

            List<KeyValuePair<int, ItemData>> rawItemData = new List<KeyValuePair<int, ItemData>>(JsonSerializer.Deserialize<dynamic>(await DataDragonUtils.GetAsync($"{version.CDN}/{version.Item}/data/en_US/item.json")).data.ToObject<Dictionary<int, ItemData>>());
            Log.Info($"Loaded {rawItemData.Count} items");

            rawItemData.ForEach(kvPair => {
                ItemData itemData = kvPair.Value;
                itemData.itemID = kvPair.Key;
                if (itemData.gold.total >= ConfigController.Component.DataDragon.MinimumGoldCost || itemData.specialRecipe != 0)
                {
                    Items.Add(itemData);
                    FullIDs.Add(itemData.itemID);
                }
            });

            Log.Info($"Registered {Items.Count} full items");

            //Download all needed champion, item, and summoner spell data
            await CheckLocalCache();

            FinishLoading.Invoke(this, EventArgs.Empty);

        }

        public Champion GetChampionById(int champID)
        {
            var champData = Champions.Find(c => c.key == champID);
            if (champData != null)
                DataDragonUtils.ExtendChampionLocal(champData, version);
            return champData;
        }

        public SummonerSpell GetSummonerById(int summonerID)
        {
            var summonerData = SummonerSpells.Find(s => s.key == summonerID);
            if (summonerData != null)
                DataDragonUtils.ExtendSummonerLocal(summonerData, version);
            return summonerData;
        }

        public ItemData GetItemById(int itemID)
        {
            var itemData = Items.Find(i => i.itemID == itemID);
            if (itemData != null)
                DataDragonUtils.ExtendItemLocal(itemData, version);
            return itemData;
        }

        public async Task<bool> CheckLocalCache()
        {
            Status = "Checking Cache";
            Log.Verbose("Checking Local Cache");

            var patch = version.Champion;
            var dlTasks = new List<Task>();

            string path = currentDir;
            string cache = path + "/Cache";
            string patchFolder = cache + "/" + ((string)patch);
            string champ = patchFolder + "/champion";
            string item = patchFolder + "/item";
            string spell = patchFolder + "/spell";

            if (!Directory.Exists(cache))
                Directory.CreateDirectory(cache);

            if (!Directory.Exists(patchFolder))
            {
                //Delete old patch folders
                Log.Info("Current Patch cache not detected, removing old Patch data");
                Status = "Yeeting old patch onto Dominion map";

                List<string> dirs = new List<string>(Directory.EnumerateDirectories(cache));

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
                if(Directory.GetFiles(champ).Length < Champions.Count * 4)
                {
                    DownloadMissingChampionCache(dlTasks, champ);
                }
                if(Directory.GetFiles(item).Length < Items.Count)
                {
                    DownloadMissingItemCache(dlTasks, item);
                }
                if(Directory.GetFiles(spell).Length < SummonerSpells.Count)
                {
                    DownloadMissingSummonerSpellCache(dlTasks, spell);
                } 
                return true;
            }

            if (!Directory.Exists(champ))
                Directory.CreateDirectory(champ);

            if (!Directory.Exists(item))
                Directory.CreateDirectory(item);

            if (!Directory.Exists(spell))
                Directory.CreateDirectory(spell);

            Log.Info("Starting download process. This could take a while");

            DownloadFullItemCache(dlTasks, champ);
            DownloadFullChampionCache(dlTasks, item);
            DownloadFullSummonerSpellCache(dlTasks, spell);

            //LoadingProgress.LoadingPopUp.IsVisible = true;
            //LoadingProgress.LoadingPopUp.UpdateProgress(0, dlTasks.Count);

            Log.Info($"Downloading {dlTasks.Count} assets from datadragon!");
            dlTasks.ForEach(t => t.Start());
            var totalTasks = dlTasks.Count;

            Status = $"Downloaded 0/{totalTasks} Images";
            int total = 0;
            while (dlTasks.Any())
            {
                Task finishTask = await Task.WhenAny(dlTasks);
                dlTasks.Remove(finishTask);
                total++;
                //LoadingProgress.LoadingPopUp.UpdateProgress(total, totalTasks);
                Status = $"Downloaded {total}/{totalTasks} Images";
            }

            Log.Info("DataDragon download finished");
            Status = "DataDragon cache downloaded";
            return true;

        }

        private void DownloadFullChampionCache(List<Task> dlTasks, string destUri)
        {
            Log.Info($"Downloading Champion Cache of {Champions.Count * 4} images");
            Champions.ForEach(findChampion => {
                DownloadChampionToCache(dlTasks, destUri, findChampion);
            });
        }

        private void DownloadMissingChampionCache(List<Task> dlTasks, string destUri)
        {
            Champions.ForEach(champ => {
                Status = $"Checking {champ} cache";
                DataDragonUtils.ExtendChampion(champ, version);
                if (!File.Exists($"{destUri}/{champ.id}_loading.png"))
                    dlTasks.Add(new Task(() => DataDragonUtils.DownloadFile(champ.loadingImg, $"{destUri}/{champ.id}_loading.png")));
                if(!File.Exists($"{destUri}/{champ.id}_splash.png"))
                    dlTasks.Add(new Task(() => DataDragonUtils.DownloadFile(champ.splashImg, $"{destUri}/{champ.id}_splash.png")));
                if(!File.Exists($"{destUri}/{champ.id}_centered_splash.png"))
                    dlTasks.Add(new Task(() => DataDragonUtils.DownloadFile(champ.splashCenteredImg, $"{destUri}/{champ.id}_centered_splash.png")));
                if(!File.Exists($"{destUri}/{champ.id}_square.png"))
                    dlTasks.Add(new Task(() => DataDragonUtils.DownloadFile(champ.squareImg, $"{destUri}/{champ.id}_square.png")));
            });
        }

        private void DownloadChampionToCache(List<Task> dlTasks, string destUri, Champion champ)
        {
            DataDragonUtils.ExtendChampion(champ, version);
            dlTasks.Add(new Task(() => DataDragonUtils.DownloadFile(champ.loadingImg, $"{destUri}/{champ.id}_loading.png")));
            dlTasks.Add(new Task(() => DataDragonUtils.DownloadFile(champ.splashImg, $"{destUri}/{champ.id}_splash.png")));
            dlTasks.Add(new Task(() => DataDragonUtils.DownloadFile(champ.splashCenteredImg, $"{destUri}/{champ.id}_centered_splash.png")));
            dlTasks.Add(new Task(() => DataDragonUtils.DownloadFile(champ.squareImg, $"{destUri}/{champ.id}_square.png")));
        }

        private void DownloadFullItemCache(List<Task> dlTasks, string destUri)
        {
            Log.Info($"Downloading Item Cache of {Items.Count} images");
            Items.ForEach(findItem =>
            {
                DownloadItemToCache(dlTasks, destUri, findItem);
            });
        }

        private void DownloadMissingItemCache(List<Task> dlTasks, string destUri)
        {
            Items.ForEach(item => {
                Status = $"Checking {item} cache";
                if (!File.Exists($"{destUri}/{item.itemID}.png"))
                    DownloadItemToCache(dlTasks, destUri, item);
            });
        }

        private void DownloadItemToCache(List<Task> dlTasks, string destUri, ItemData item)
        {
            DataDragonUtils.ExtendItem(item, version);
            dlTasks.Add(new Task(() => DataDragonUtils.DownloadFile(item.sprite, $"{destUri}/{item.itemID}.png")));
        }
        
        private void DownloadFullSummonerSpellCache(List<Task> dlTasks, string destUri)
        {
            Log.Info($"Downloading Summoner Spell Cache of {SummonerSpells.Count} images");
            SummonerSpells.ForEach(findSummoner => {
                DownloadSummonerSpellToCache(dlTasks, destUri, findSummoner);
            });
        }

        private void DownloadMissingSummonerSpellCache(List<Task> dlTasks, string destUri)
        {
            SummonerSpells.ForEach(spell =>
            {
                Status = $"Checking {spell} cache";
                if (!File.Exists($"{destUri}/{spell.id}.png"))
                    DownloadSummonerSpellToCache(dlTasks, destUri, spell);
            });
        }

        private void DownloadSummonerSpellToCache(List<Task> dlTasks, string destUri, SummonerSpell spell)
        {
            DataDragonUtils.ExtendSummoner(spell, version);
            dlTasks.Add(new Task(() => DataDragonUtils.DownloadFile(spell.icon, $"{destUri}/{spell.id}.png")));
        }
    }

    static class DataDragonUtils
    {
        public static void Empty(this DirectoryInfo directory)
        {
            foreach (FileInfo file in directory.EnumerateFiles()) file.Delete();
            foreach (DirectoryInfo subDirectory in directory.EnumerateDirectories()) subDirectory.Delete(true);
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
            client.DownloadFile(uri, path);
            client.Dispose();
        }

        public static async Task<string> GetAsync(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            using Stream stream = response.GetResponseStream();
            using StreamReader reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }

        public static void ExtendChampion(Champion champion, GameVersion version)
        {
            champion.splashImg = $"{version.CDN}/img/champion/splash/{ champion.id}_0.jpg";
            champion.splashCenteredImg = $"https://cdn.communitydragon.org/{version.Version}/champion/{champion.id}/splash-art/centered";
            champion.squareImg = $"{version.GetVersionCDN()}/img/champion/{ champion.id}.png";
            champion.loadingImg = $"{version.CDN}/img/champion/loading/{ champion.id}_0.jpg";
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