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
using Swan.Logging;
using LeagueBroadcast.Common.Utils;
using LeagueBroadcast.ChampSelect.Data.DTO;
using LeagueBroadcast.Update.Http;
using System.Text.Json;
using LeagueBroadcast.Common.Data.Config;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace LeagueBroadcast.Common.Data.Provider
{
    public class FileLoadProgressEventArgs
    {
        public string FileName { get; set; } = "";

        public string Task { get; set; } = "";
        public int Completed { get; set; }
        public int Total { get; set; }


        public FileLoadProgressEventArgs(string fileName, string task, int completed, int total)
        {
            FileName = fileName;
            Task = task;
            Completed = completed;
            Total = total;
        }
    }

    class DataDragon : ObservableObject
    {
        public static readonly string currentDir = Directory.GetCurrentDirectory();
        private static DataDragon _instance;
        public static DataDragon Instance => GetInstance();

        public static GameVersion version;



        private static TaskCompletionSource<bool>? _downloadComplete;

        private static int _toDownload, _downloaded;
        private static int IncrementToDownload() => Interlocked.Increment(ref _toDownload);
        private static int IncrementToDownload(int count) => Interlocked.Add(ref _toDownload, count);
        private static int IncrementDownloaded() => Interlocked.Increment(ref _downloaded);
        private static int IncrementDownloaded(int count) => Interlocked.Add(ref _downloaded, count);

        public static EventHandler FinishLoading, StartLoading;
        public static EventHandler<FileLoadProgressEventArgs>? FileDownloadComplete { get; set; }


        private StartupViewModel _startupContext = (StartupViewModel)BroadcastController.Instance.Startup.DataContext;

        public struct GameVersion
        {
            public StringVersion localVersion;
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

            Log.Info("[CDrag] CommunityDragon Provider Init");
            _startupContext.Status = "CommunityDragon Init";
            version = new GameVersion
            {
                Version = ConfigController.Component.DataDragon.Patch,
                CDN = ConfigController.Component.DataDragon.CDN

            };


            StartLoading?.Invoke(this, EventArgs.Empty);
            if (version.Version == "latest")
            {
                Log.Info("[CDrag] Getting latest versions");

                new Task(async () =>
                {
                    await GetLatestGameVersion();
                    await Init();
                }).Start();
            }
            else
            {

                Log.Info($"Using version from configuration: {version.Version}");
                version.Champion = version.Version;
                version.Item = version.Version;
                version.Summoner = version.Version;
                new Task(() => { Init(); }).Start();


                Log.Info($"[CDrag] Using version from configuration: {version.Version}");

                new Task(async () =>
                {
                    if (!StringVersion.TryParse(version.Version, out StringVersion? requestedPatch))
                    {
                        "[CDrag] Could not read requested game version. Falling back to latest".Warn();
                        await GetLatestGameVersion();
                    }
                    await Init();
                }).Start();
            }
        }


        private static async Task GetLatestGameVersion()
        {
            GetInstance()._startupContext.Status = "Retrieving latest patch info";
            Log.Info("[CDrag] Retrieving latest patch info");

            string? rawCDragVersionResponse = JsonDocument.Parse(await RestRequester.GetRaw($"{ConfigController.Component.DataDragon.CDragonRaw}/latest/content-metadata.json")??"").RootElement.GetProperty("version").GetString();
            if (rawCDragVersionResponse is null)
            {
                Log.Warn($"[CDrag] {$"{ConfigController.Component.DataDragon.CDragonRaw}/latest/content-metadata.json"} unreachable. Is your internet connection working?");
                Log.Warn($"[CDrag] Could not get latest CDragon version. Falling back to latest DDrag version");
                using JsonDocument response = JsonDocument.Parse(await RestRequester.GetRaw($"https://ddragon.leagueoflegends.com/realms/{ConfigController.Component.DataDragon.Region}.json")??"", new JsonDocumentOptions { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip });
                rawCDragVersionResponse = response.RootElement.GetProperty("n").GetProperty("champion").ToString();

                if(rawCDragVersionResponse is null)
                {
                    Log.Warn($"[CDrag] DDrag not reachable. Assuming internet connection issues. Cannot retrieve data");
                    return;
                }
            }
            StringVersion localVersion = StringVersion.Parse(rawCDragVersionResponse.Split("+")[0]);
            version.localVersion = localVersion;
            version.CDN = localVersion.ToString();
            version.Champion = localVersion.ToString();
            version.Item = localVersion.ToString();
            version.Summoner = localVersion.ToString();

            Log.Info($"[CDrag] Using live patch {version.CDN} on platform {ConfigController.Component.DataDragon.Region}");
            Log.Info($"{localVersion.ToString(2)}, {GetLatestLocalPatch().ToString(2)}");

            if (StringVersion.Parse(localVersion.ToString(2)) > StringVersion.Parse(GetLatestLocalPatch().ToString(2)))
            {
                Log.Info($"[CDrag] New patch {version.CDN} detected");
            }
        }


        private static StringVersion GetLatestLocalPatch()
        {
            string patchDir = Path.Combine(currentDir, "Cache");
            if (Directory.Exists(patchDir))
            {
                $"Found cache folder".Debug();
                return Directory.GetDirectories(patchDir).Select(Path.GetFileName).Where(dir => dir!.Count(c => c == '.') == 2).Select(dir => StringVersion.Parse(dir)).Max() ?? StringVersion.Zero;
            }

            return new(0, 0, 0);
        }

        private static async Task Init()
        {

            CDragonChampion.All = (await RestRequester.GetAsync<HashSet<CDragonChampion>>($"{ConfigController.Component.DataDragon.CDragonRaw}/{version.localVersion.ToString(2)}/plugins/rcp-be-lol-game-data/{ConfigController.Component.DataDragon.Region}/default/v1/champion-summary.json")).Where(c => c.ID > 0).ToHashSet();
            Log.Info($"[CDrag] Loaded {CDragonChampion.All.Count} champions");

            SummonerSpell.All = await RestRequester.GetAsync<HashSet<SummonerSpell>>($"{ConfigController.Component.DataDragon.CDragonRaw}/{version.localVersion.ToString(2)}/plugins/rcp-be-lol-game-data/{ConfigController.Component.DataDragon.Region}/default/v1/summoner-spells.json");
            Log.Info($"[CDrag] Loaded {SummonerSpell.All.Count} summoner spells");

            CDragonItem.All = await RestRequester.GetAsync<HashSet<CDragonItem>>($"{ConfigController.Component.DataDragon.CDragonRaw}/{version.localVersion.ToString(2)}/plugins/rcp-be-lol-game-data/{ConfigController.Component.DataDragon.Region}/default/v1/items.json");
            Log.Info($"[CDrag] Loaded {CDragonItem.All.Count} items");

            CDragonItem.Full = CDragonItem.All.Where(item => item.PriceTotal > ConfigController.Component.DataDragon.MinimumGoldCost).ToHashSet();
            Log.Info($"[CDrag] Loaded {CDragonItem.Full.Count} full items");

            bool result = await VerifyLocalCache(version.localVersion);

            FinishLoading?.Invoke(null, EventArgs.Empty);
        }

        private static async Task<bool> VerifyLocalCache(StringVersion currentPatch)
        {
            GetInstance()._startupContext.Status = "Verifying local cache";

            _downloadComplete = new TaskCompletionSource<bool>();

            currentPatch = StringVersion.Parse($"{currentPatch.ToString(2)}.1");
            string cache = currentDir + "/Cache";
            string patchFolder = cache + $"/{currentPatch}";
            string champ = patchFolder + "/champion";
            string item = patchFolder + "/item";
            string spell = patchFolder + "/spell";

            _ = Directory.CreateDirectory(cache);
            _ = Directory.CreateDirectory(patchFolder);
            _ = Directory.CreateDirectory(champ);
            _ = Directory.CreateDirectory(item);
            _ = Directory.CreateDirectory(spell);

            GetInstance()._startupContext.Status = "Yeeting old caches onto Dominion";
            Directory.EnumerateDirectories(cache).Where(d => StringVersion.TryParse(d.Split("/")[^1].Split("\\")[^1], out StringVersion? dirVersion) && dirVersion < currentPatch).ToList().ForEach(dir =>
            {
                new DirectoryInfo(dir).Empty();
                Directory.Delete(dir);
                Log.Info($"Removed Patch Cache {dir}");
            });

            ConcurrentBag<string[]> failedDownloads = new();
            int toCache = IncrementToDownload(CDragonChampion.All.Count * 4 + CDragonItem.All.Count + SummonerSpell.All.Count);

            Stopwatch s = new();
            s.Start();

            await DownloadMissingChampionCache(champ, failedDownloads);
            DownloadMissingItemCache(item, failedDownloads);
            DownloadMissingSummonerSpellCache(spell, failedDownloads);

            s.Stop();
            $"[CDrag] Verified local cache in {s.ElapsedMilliseconds}ms".Debug();

            if (_toDownload == toCache)
            {
                Log.Info("Local cache up to date");
                return true;
            }

            Log.Info($"[CDrag] Downloaded {_toDownload} assets from CommunityDragon");

            if (_downloaded == _toDownload)
            {
                _ = _downloadComplete.TrySetResult(failedDownloads.IsEmpty);
            }

            bool updateResult = await _downloadComplete.Task;
            Log.Info($"[CDrag] Downloaded missing assets");

            return updateResult;
        }

        private static async Task DownloadMissingChampionCache(string location, ConcurrentBag<string[]> failedDownloads)
        {
            Log.Info("[CDrag] Verifying Champion Assets");

            foreach (CDragonChampion champ in CDragonChampion.All)
            {
                //Check if all files for the champ exist
                bool loadingExists = File.Exists($"{location}/{champ.Alias}_loading.png");
                bool splashExists = File.Exists($"{location}/{champ.Alias}_splash.png");
                bool centeredSplashExists = File.Exists($"{location}/{champ.Alias}_centered_splash.png");
                bool squareExists = File.Exists($"{location}/{champ.Alias}_square.png");

                if (loadingExists && splashExists && centeredSplashExists && squareExists)
                {
                    ExtendChampionLocal(champ, version.localVersion);
                    FileDownloadComplete?.Invoke(null, new FileLoadProgressEventArgs(champ.Alias, "Verified", IncrementDownloaded(4), _toDownload));
                    continue;
                }

                //Get champ data if not all files exist
                await ExtendChampion(champ, version.localVersion.ToString(2));

                //Get missing files
                if (!loadingExists)
                {
                    DownloadAsset(champ.LoadingImg!, $"{location}/{champ.Alias}_loading.png", $"{champ.Alias}_loading.png", failedDownloads);
                }

                if (!splashExists)
                {
                    DownloadAsset(champ.SplashImg!, $"{location}/{champ.Alias}_splash.png", $"{champ.Alias}_splash.png", failedDownloads);
                }

                if (!centeredSplashExists)
                {
                    DownloadAsset(champ.SplashCenteredImg!, $"{location}/{champ.Alias}_centered_splash.png", $"{champ.Alias}_centered_splash.png", failedDownloads);
                }

                if (!squareExists)
                {
                    DownloadAsset(champ.SquareImg!, $"{location}/{champ.Alias}_square.png", $"{champ.Alias}_square.png", failedDownloads);
                }

                ExtendChampionLocal(champ, version.localVersion);

                FileDownloadComplete?.Invoke(null, new FileLoadProgressEventArgs(champ.Alias, "Verified", IncrementDownloaded(4), _toDownload));
            };

            Log.Info($"[CDrag] Verified all champion assets");
        }


        private static void DownloadMissingItemCache(string location, ConcurrentBag<string[]> failedDownloads)
        {
            Log.Info("[CDrag] Verifying Item Assets");

            string stringVersion = version.localVersion.ToString(2);
            foreach (CDragonItem item in CDragonItem.All)
            {
                if (!File.Exists($"{location}/{item.ID}.png"))
                {
                    DownloadAsset($"{ConfigController.Component.DataDragon.CDragonRaw}/{stringVersion}/plugins/rcp-be-lol-game-data/{ConfigController.Component.DataDragon.Region}/default/assets/items/icons2d/{item.IconPath.Split("/")[^1].ToLower()}", $"{location}/{item.ID}.png", $"{item.ID}.png", failedDownloads);
                }
                ExtendItemLocal(item, version.localVersion);
                FileDownloadComplete?.Invoke(null, new FileLoadProgressEventArgs(item.Name, "Verified", IncrementDownloaded(), _toDownload));
            };
        }

        private static void DownloadMissingSummonerSpellCache(string location, ConcurrentBag<string[]> failedDownloads)
        {
            Log.Info("[CDrag] Verifying Summoner Spell Assets");
            string stringVersion = version.localVersion.ToString(2);
            foreach (SummonerSpell spell in SummonerSpell.All)
            {
                if (!File.Exists($"{location}/{spell.ID}.png"))
                {
                    DownloadAsset($"{ConfigController.Component.DataDragon.CDragonRaw}/{stringVersion}/plugins/rcp-be-lol-game-data/{ConfigController.Component.DataDragon.Region}/default/data/spells/icons2d/{spell.IconPath.Split("/")[^1].ToLower()}", $"{location}/{spell.ID}.png", $"{spell.ID}.png", failedDownloads);
                }
                ExtendSummonerLocal(spell, version.localVersion);
                FileDownloadComplete?.Invoke(null, new FileLoadProgressEventArgs(spell.Name, "Verified", IncrementDownloaded(), _toDownload));
            };
        }

        private static void DownloadAsset(string remote, string local, string fileName, ConcurrentBag<string[]> failedDownloads)
        {
            _ = IncrementToDownload();
            $"Downloading {fileName} from {remote}".Debug();
            Task t = Task.Run(async () =>
            {
                System.Net.HttpStatusCode res = await FileDownloader.DownloadAsync(remote, local);
                if (res == System.Net.HttpStatusCode.OK)
                {
                    $"{fileName} downloaded".Debug();
                }
                else
                {
                    failedDownloads.Add(new string[] { remote, fileName });
                    $"Download {fileName} from {remote} to {local} failed: {res}".Debug();
                }

                FileDownloadComplete?.Invoke(null, new FileLoadProgressEventArgs(fileName, "Downloaded", IncrementDownloaded(), _toDownload));

                if (_downloaded >= _toDownload)
                {
                    _ = _downloadComplete!.TrySetResult(true);
                }
            });
        }

        #region ObjectExtension
        public static async Task ExtendChampion(CDragonChampion champion, string version)
        {
            using JsonDocument response = JsonDocument.Parse(await RestRequester.GetRaw($"{ConfigController.Component.DataDragon.CDragonRaw}/{version}/plugins/rcp-be-lol-game-data/{ConfigController.Component.DataDragon.Region}/default/v1/champions/{champion.ID}.json"), new JsonDocumentOptions { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip });
            JsonElement root = response.RootElement;
            JsonElement defaultSkin = root.GetProperty("skins").EnumerateArray().Single(skin => $"{skin.GetProperty("id").GetInt32()}" == $"{champion.ID}000");

            champion.SplashImg = $"{ConfigController.Component.DataDragon.CDragonRaw}/{version}/plugins/rcp-be-lol-game-data/{ConfigController.Component.DataDragon.Region}/default/v1/champion-splashes/uncentered/{champion.ID}/{(defaultSkin.GetProperty("uncenteredSplashPath").GetString() ?? "").Split("/")[^1]}";
            champion.SplashCenteredImg = $"{ConfigController.Component.DataDragon.CDragonRaw}/{version}/plugins/rcp-be-lol-game-data/{ConfigController.Component.DataDragon.Region}/default/v1/champion-splashes/{champion.ID}/{(defaultSkin.GetProperty("splashPath").GetString() ?? "").Split("/")[^1]}";
            champion.SquareImg = $"{ConfigController.Component.DataDragon.CDragonRaw}/{version}/plugins/rcp-be-lol-game-data/{ConfigController.Component.DataDragon.Region}/default/v1/champion-icons/{(root.GetProperty("squarePortraitPath").GetString() ?? "").Split("/")[^1]}";
            champion.LoadingImg = $"{ConfigController.Component.DataDragon.CDragonRaw}/{version}/plugins/rcp-be-lol-game-data/{ConfigController.Component.DataDragon.Region}/default/assets/characters/{champion.Alias.ToLower()}/skins/base/{(defaultSkin.GetProperty("loadScreenPath").GetString() ?? "").Split("/")[^1].ToLower()}";
        }

        public static void ExtendChampionLocal(CDragonChampion champion, StringVersion version)
        {
            string championPath = $"cache/{version.ToString(2)}.1/champion";
            champion.SplashImg = $"{championPath}/{champion.Alias}_splash.jpg";
            champion.SplashCenteredImg = $"{championPath}/{champion.Alias}_centered_splash.png";
            champion.SquareImg = $"{championPath}/{champion.Alias}_square.png";
            champion.LoadingImg = $"{championPath}/{champion.Alias}_loading.png";
        }

        public static void ExtendSummonerLocal(SummonerSpell summoner, StringVersion version)
        {
            summoner.IconPath = Path.Combine("cache", $"{version.ToString(2)}.1", "spell", $"{summoner.Name}.png");
        }

        public static void ExtendItemLocal(CDragonItem item, StringVersion version)
        {
            item.IconPath = Path.Combine("cache", $"{version.ToString(2)}.1", "item", item.ID + ".png");
        }

        #endregion


        public Champion GetChampionById(int champID)
        {
            var champData =  CDragonChampion.All.SingleOrDefault(c => c.ID == champID);
            if (champData is null)
                return new Champion();
            return new Champion()
            {
                id = champData.Alias,
                key = champData.ID,
                name = champData.Name,
                loadingImg = champData.LoadingImg,
                splashCenteredImg = champData.SplashCenteredImg,
                splashImg = champData.SplashImg,
                squareImg = champData.SquareImg
            };
        }

        public SummonerSpell GetSummonerById(int summonerID)
        {
            return SummonerSpell.All.SingleOrDefault(s => s.ID == summonerID);
        }

        public ItemData GetItemById(int itemID)
        {
            var itemData = CDragonItem.All.SingleOrDefault(i => i.ID == itemID);

            return new ItemData(itemData.ID)
            {
                itemID = itemData.ID,
                name = itemData.Name,
                specialRecipe = itemData.SpecialRecipe,
                sprite = itemData.IconPath,
                gold = new ItemCost()
                {
                    sell = 0,
                    total = itemData.PriceTotal
                }
            };
        }
    }
}