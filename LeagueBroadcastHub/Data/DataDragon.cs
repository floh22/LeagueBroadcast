using LeagueBroadcastHub.Data.Containers;
using LeagueIngameServer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static LeagueBroadcastHub.Data.DataDragon;

namespace LeagueBroadcastHub.Data
{
    class DataDragon
    {
        public static readonly string currentDir = Directory.GetCurrentDirectory();
        public static DataDragon Instance;

        const string Realm = "euw";
        GameVersion version;
        List<Champion> Champions;
        List<SummonerSpell> SummonerSpells;
        List<ItemData> Items;

        public List<int> FullIDs;

        public EventHandler FinishedLoading;

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
        public DataDragon()
        {
            if (Instance == null)
                Instance = this;
            else
                return;

            version = new GameVersion
            {
                Version = Properties.Settings.Default.Version,
                CDN = Properties.Settings.Default.CDN
            };

            Champions = new List<Champion>();
            SummonerSpells = new List<SummonerSpell>();
            Items = new List<ItemData>();
            FullIDs = new List<int>();


            if(version.Version == "latest")
            {
                System.Diagnostics.Debug.WriteLine("Getting latest versions from dataDragon");
                new Task(() => { 
                    InitLatest(); 
                }).Start();
            } else
            {
                System.Diagnostics.Debug.WriteLine($"Using version from configuration: {version.Version}");
                version.Champion = version.Version;
                version.Item = version.Version;
                version.Summoner = version.Version;
                new Task(() => { Init(); }).Start();
            }
        }

        private async void InitLatest()
        {
            dynamic riotVersion = JsonConvert.DeserializeObject<dynamic>(await DataDragonUtils.GetAsync($"https://ddragon.leagueoflegends.com/realms/{Realm}.json"));
            version.CDN = riotVersion.cdn;
            version.Champion = riotVersion.n.champion;
            version.Item = riotVersion.n.item;
            version.Summoner = riotVersion.n.summoner;

            Init();
        }

        private async void Init()
        {
            System.Diagnostics.Debug.WriteLine($"Champion: {version.Champion}, Item: {version.Item}, CDN: {version.CDN}");

            Champions = new List<Champion>(JsonConvert.DeserializeObject<dynamic>(await DataDragonUtils.GetAsync($"{version.CDN}/{version.Champion}/data/en_US/champion.json")).data.ToObject<Dictionary<string, Champion>>().Values);
            System.Diagnostics.Debug.WriteLine($"Loaded {Champions.Count} champions");

            SummonerSpells = new List<SummonerSpell>(JsonConvert.DeserializeObject<dynamic>(await DataDragonUtils.GetAsync($"{version.CDN}/{version.Item}/data/en_US/summoner.json")).data.ToObject<Dictionary<string, SummonerSpell>>().Values);
            System.Diagnostics.Debug.WriteLine($"Loaded {SummonerSpells.Count} summoner spells");

            List<KeyValuePair<int, ItemData>> rawItemData = new List<KeyValuePair<int, ItemData>>(JsonConvert.DeserializeObject<dynamic>(await DataDragonUtils.GetAsync($"{version.CDN}/{version.Item}/data/en_US/item.json")).data.ToObject<Dictionary<int, ItemData>>());
            System.Diagnostics.Debug.WriteLine($"Loaded {rawItemData.Count} items");

            rawItemData.ForEach(kvPair => {
                ItemData itemData = kvPair.Value;
                itemData.itemID = kvPair.Key;
                if(itemData.gold.total >= Properties.Settings.Default.ItemGoldCutoff || itemData.specialRecipe != 0)
                {
                    Items.Add(itemData);
                    FullIDs.Add(itemData.itemID);
                }
            });

            System.Diagnostics.Debug.WriteLine($"Registered {Items.Count} full items");

            //Download all needed champion, item, and summoner spell data
            await CheckLocalCache();

            FinishedLoading.Invoke(this, EventArgs.Empty);

        }

        public ItemData GetItem(int itemID)
        {
            var itemData = new ItemData(itemID);
            DataDragonUtils.ExtendItemLocal(itemData, version);
            return itemData;
        }

        public Champion GetChampionById(string champID)
        {
            var champData = Champions.Find(c => c.id == champID);
            DataDragonUtils.ExtendChampionLocal(champData, version);
            return champData;
        }

        public SummonerSpell GetSummonerById(string summonerID)
        {
            var summonerData = SummonerSpells.Find(s => s.id == summonerID);
            DataDragonUtils.ExtendSummonerLocal(summonerData, version);
            return summonerData;
        }

        public ItemData GetItemById(int itemID)
        {
            var itemData = Items.Find(i => i.itemID == itemID);
            DataDragonUtils.ExtendItemLocal(itemData, version);
            return itemData;
        }

        public async Task<bool> CheckLocalCache()
        {
            var patch = version.Champion;
            var dlTasks = new List<Task>();

            string path = currentDir;
            string cache = path +  "/cache";
            string patchFolder = cache + "/" + ((string) patch);
            string champ = patchFolder +  "/champion";
            string item = patchFolder + "/item";
            string spell = patchFolder + "/spell";

            if (!Directory.Exists(cache))
                Directory.CreateDirectory(cache);

            if (!Directory.Exists(patchFolder))
            {
                //Delete old patch folders
                System.Diagnostics.Debug.WriteLine("Current Patch cache not detected, removing old Patch data");

                List<string> dirs = new List<string>(Directory.EnumerateDirectories(cache));

                dirs.ForEach(dir => {
                    new DirectoryInfo(dir).Empty();
                    Directory.Delete(dir);
                    System.Diagnostics.Debug.WriteLine($"Removed Patch Cache {dir}");
                });

                Directory.CreateDirectory(patchFolder);
            } else
            {
                System.Diagnostics.Debug.WriteLine($"Cache {patchFolder} exists already");
                return true;
            }

            if (!Directory.Exists(champ))
                Directory.CreateDirectory(champ);

            if (!Directory.Exists(item))
                Directory.CreateDirectory(item);

            if (!Directory.Exists(spell))
                Directory.CreateDirectory(spell);            

            System.Diagnostics.Debug.WriteLine("Starting download process. This could take a while");

            Items.ForEach(findItem =>
            {
                DataDragonUtils.ExtendItem(findItem, version);
                dlTasks.Add(new Task(() => DataDragonUtils.DownloadFile(findItem.sprite, $"{item}/{findItem.itemID}.png")));
            });

            Champions.ForEach(findChampion => {
                DataDragonUtils.ExtendChampion(findChampion, version);
                dlTasks.Add(new Task(() => DataDragonUtils.DownloadFile(findChampion.loadingImg, $"{champ}/{findChampion.id}_loading.png")));
                dlTasks.Add(new Task(() => DataDragonUtils.DownloadFile(findChampion.splashImg, $"{champ}/{findChampion.id}_splash.png")));
                dlTasks.Add(new Task(() => DataDragonUtils.DownloadFile(findChampion.splashCenteredImg, $"{champ}/{findChampion.id}_centered_splash.png")));
                dlTasks.Add(new Task(() => DataDragonUtils.DownloadFile(findChampion.squareImg, $"{champ}/{findChampion.id}_square.png")));
            });

            SummonerSpells.ForEach(findSummoner => {
                DataDragonUtils.ExtendSummoner(findSummoner, version);
                dlTasks.Add(new Task(() => DataDragonUtils.DownloadFile(findSummoner.icon, $"{spell}/{findSummoner.id}.png")));
            });

            //LoadingProgress.LoadingPopUp = new LoadingProgress("Downloading assets from Datadragon", 0, dlTasks.Count, true);
            LoadingProgress.LoadingPopUp.IsVisible = true;
            LoadingProgress.LoadingPopUp.UpdateProgress(0, dlTasks.Count);

            System.Diagnostics.Debug.WriteLine($"Downloading {dlTasks.Count} assets from datadragon!");
            dlTasks.ForEach(t => t.Start());
            var totalTasks = dlTasks.Count;


            int total = 0;
            while (dlTasks.Any())
            {
                Task finishTask = await Task.WhenAny(dlTasks);
                dlTasks.Remove(finishTask);
                total++;
                LoadingProgress.LoadingPopUp.UpdateProgress(total, totalTasks);
            }

            System.Diagnostics.Debug.WriteLine("Download finished");
            LoadingProgress.LoadingPopUp.IsVisible = false;

            return true;

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
            //return paths.Aggregate(Path.Combine);
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
            champion.splashCenteredImg = $"{championPath}/{champion.id}_centered_splash.jpg";
            champion.squareImg = $"{championPath}/{ champion.id}.png";
            champion.loadingImg = $"{championPath}/{ champion.id}_0.jpg";
        }

        public static void ExtendSummoner(SummonerSpell summoner, GameVersion version)
        {
            summoner.icon = $"{version.GetVersionCDN()}/img/spell/{summoner.id}.png";
        }

        public static void ExtendSummonerLocal(SummonerSpell summoner, GameVersion version)
        {
            summoner.icon = CombinePaths("cache", version.Item, "summoner", $"{summoner.id}.png");
        } 

        public static void ExtendItem(ItemData item, GameVersion version)
        {
            item.sprite = $"{version.GetVersionCDN()}/img/item/{item.itemID}.png";
        }

        public static void ExtendItemLocal(ItemData item, GameVersion version)
        {
            item.sprite = CombinePaths("cache", version.Item, "item", item.itemID + ".png");
            //item.sprite = $"{ itemsPath}/{item.itemID}.png";
        }
    }

}
