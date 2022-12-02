using Common.Http;
using LeagueBroadcast.Common.Config;
using LeagueBroadcast.Common.Data.CommunityDragon;
using LeagueBroadcast.Common.Data.LCU;
using LeagueBroadcast.Common.Events;
using LeagueBroadcast.Utils;
using LeagueBroadcast.Utils.Http;
using LeagueBroadcast.Utils.Log;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;

namespace LeagueBroadcast.Server.DataProvider
{
    public static class CommunityDragonDataProvider
    {
        private static readonly string _currentDir = Path.Combine(WorkingDirectory.GetDirectoryAsync().Result, "Data");

        private static int _toDownload, _downloaded;
        private static TaskCompletionSource<bool>? _downloadComplete;
        private static CommunityDragonConfig? _cfg;

        private static int IncrementToDownload() => Interlocked.Increment(ref _toDownload);
        private static int IncrementToDownload(int count) => Interlocked.Add(ref _toDownload, count);
        private static int IncrementDownloaded() => Interlocked.Increment(ref _downloaded);
        private static int IncrementDownloaded(int count) => Interlocked.Add(ref _downloaded, count);


        public static async void Startup()
        {
            "CommunityDragon Data Provider Start".Info("CDrag");
            "CommunityDragon Init".UpdateStartupProgressText();

            Stopwatch s = new();
            s.Start();

            _cfg = ConfigController.Get<ComponentConfig>().CommunityDragon;
            StringVersion gameVersion;
            if (_cfg.Patch == "latest")
            {
                gameVersion = await GetLatestGameVersion();
            }
            else
            {
                $"Using version from configuration: {_cfg.Patch}".Info("CDrag");

                if (StringVersion.TryParse(_cfg.Patch, out var requestedPatch))
                {
                    gameVersion = requestedPatch!;
                }
                else
                {
                    "Could not read requested game version. Falling back to latest".Warn("CDrag");
                    gameVersion = await GetLatestGameVersion();
                }

            }

            gameVersion = StringVersion.Parse($"{gameVersion.ToString(2)}.1");

            StringVersion.SetLeagueVersion(gameVersion);


            $"Using League Data Version {gameVersion}".Info("CDrag");

            bool dataProviderLoadedSuccessfully = true;

            if(!await LoadCommunityDragonData() ||!await VerifyAndDownloadCache())
            {
                $"Could not start Community Dragon Data Provider! Check your install, and report your issue to GitHub".Error("CDrag");
                dataProviderLoadedSuccessfully = false;
            }

            s.Stop();

            CommunityDragonEventHandler.LoadComplete?.Invoke(null, new CommunityDragonDataProviderLoadedEventArgs(dataProviderLoadedSuccessfully, s.Elapsed));
        }


        private static async Task<StringVersion> GetLatestGameVersion()
        {
            "Retrieving latest patch info".Info("CDrag");
            "Retrieving latest patch info".UpdateStartupProgressText();

            string? rawCDragVersionResponse = JsonDocument.Parse(await RestRequester.GetRaw($"{_cfg!.CDragonRaw}/latest/content-metadata.json")).RootElement.GetProperty("version").GetString();
            if (rawCDragVersionResponse is null)
            {
                $"Could not get latest CDragon version. Falling back to latest DDrag version".Warn("CDrag");
                using JsonDocument response = JsonDocument.Parse(await RestRequester.GetRaw($"https://ddragon.leagueoflegends.com/realms/{_cfg.Region}.json"), new JsonDocumentOptions { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip });
                rawCDragVersionResponse = response.RootElement.GetProperty("n").GetProperty("champion").ToString();

                if (rawCDragVersionResponse is null)
                {
                    $"DDrag not reachable. Assuming internet connection issues. Cannot retrieve data. Manually set version instead of latest".Error("CDrag");
                    return StringVersion.Zero;
                }


            }
            StringVersion latestGameVersion = StringVersion.Parse(rawCDragVersionResponse.Split("+")[0]);

            $"Detected live patch {latestGameVersion} on platform {_cfg.Region}".Info("CDrag");

            if (StringVersion.Parse(latestGameVersion.ToString(2)) > StringVersion.Parse(GetLatestLocalPatch().ToString(2)))
            {
                $"New patch detected".Info("CDrag");
            }

            return latestGameVersion;
        }

        private static StringVersion GetLatestLocalPatch()
        {
            string patchDir = Path.Combine(_currentDir, "Cache");
            if (Directory.Exists(patchDir))
            {
                $"Found cache folder".Debug("CDrag");
                return Directory.GetDirectories(patchDir).Select(Path.GetFileName).Where(dir => dir!.Count(c => c == '.') == 2).Select(dir => StringVersion.Parse(dir)).Max() ?? StringVersion.Zero;
            }

            return StringVersion.Zero;
        }

        private static async Task<bool> LoadCommunityDragonData()
        {
            try
            {
                Champion.All = (await RestRequester.GetAsync<HashSet<Champion>>($"{_cfg!.CDragonRaw}/{StringVersion.LeagueVersion.ToString(2)}/plugins/rcp-be-lol-game-data/{_cfg.Region}/default/v1/champion-summary.json") ?? new HashSet<Champion>()).Where(c => c.ID > 0).ToHashSet();
                $"Loaded {Champion.All.Count} champions".Info("CDrag");

                SummonerSpell.All = await RestRequester.GetAsync<HashSet<SummonerSpell>>($"{_cfg.CDragonRaw}/{StringVersion.LeagueVersion.ToString(2)}/plugins/rcp-be-lol-game-data/{_cfg.Region}/default/v1/summoner-spells.json") ?? new HashSet<SummonerSpell>();
                $"Loaded {SummonerSpell.All.Count} summoner spells".Info("CDrag");

                Item.All = await RestRequester.GetAsync<HashSet<Item>>($"{_cfg.CDragonRaw}/{StringVersion.LeagueVersion.ToString(2)}/plugins/rcp-be-lol-game-data/{_cfg.Region}/default/v1/items.json") ?? new HashSet<Item>();
                $"Loaded {Item.All.Count} items".Info("CDrag");

                Item.Full = Item.All.Where(item => item.PriceTotal > _cfg.MinimumItemGoldCost).ToHashSet();
                $"Loaded {Item.Full.Count} full items".Info("CDrag");

                return true;
            } catch (Exception e)
            {
                $"Could not load CDragon Data".Error("CDrag");
                $"{e.Source}: {e.Message}\nStacktrace: {e.StackTrace}".Error();

                return false;
            }
        } 

        private static async Task<bool> VerifyAndDownloadCache()
        {
            "Verifying and Updating local cache".UpdateStartupProgressText();

            _downloadComplete = new TaskCompletionSource<bool>();

            StringVersion minorPatch = StringVersion.Parse($"{StringVersion.LeagueVersion.ToString(2)}.1");
            string cache = _currentDir + "/Cache";
            string patchFolder = cache + $"/{minorPatch}";
            string champ = patchFolder + "/champion";
            string item = patchFolder + "/item";
            string spell = patchFolder + "/spell";


            _ = Directory.CreateDirectory(cache);
            _ = Directory.CreateDirectory(patchFolder);
            _ = Directory.CreateDirectory(champ);
            _ = Directory.CreateDirectory(item);
            _ = Directory.CreateDirectory(spell);

            "Yeeting old caches onto Dominion".UpdateStartupProgressText();
            Directory.EnumerateDirectories(cache).Where(d => StringVersion.TryParse(d.Split("/")[^1].Split("\\")[^1], out StringVersion? dirVersion) && dirVersion < StringVersion.LeagueVersion).ToList().ForEach(dir =>
            {
                new DirectoryInfo(dir).Empty();
                Directory.Delete(dir);
                $"Removed Patch Cache {dir}".Info("CDrag");
            });


            ConcurrentBag<string[]> failedDownloads = new();
            int toCache = IncrementToDownload(Champion.All.Count * 4 + Item.All.Count + SummonerSpell.All.Count);

            Stopwatch s = new();
            s.Start();

            try
            {
                await DownloadMissingChampionCache(champ, failedDownloads);
                DownloadMissingItemCache(item, failedDownloads);
                DownloadMissingSummonerSpellCache(spell, failedDownloads);
            } catch (Exception e)
            {
                s.Stop();
                $"Could not local cache after {s.ElapsedMilliseconds}ms".Error("CDrag");
                $"{e.Source}: {e.Message}\nStacktrace: {e.StackTrace}".Error();
                return false;
            }

            s.Stop();
            $"Verified local cache in {s.ElapsedMilliseconds}ms".Info("CDrag");

            if (_toDownload == toCache)
            {
                "Local cache up to date".Info("CDrag");
                return true;
            }

            $"Downloaded {_toDownload} assets from CommunityDragon".Info("CDrag");

            if (_downloaded == _toDownload)
            {
                _ = _downloadComplete.TrySetResult(failedDownloads.IsEmpty);
            }

            bool updateResult = await _downloadComplete.Task;
            $"Downloaded missing assets".Info("CDrag");

            return true;
        }


        private static async Task DownloadMissingChampionCache(string location, ConcurrentBag<string[]> failedDownloads)
        {
            "Verifying Champion Assets".Info("CDrag");

            int count = 0;

            foreach (Champion champ in Champion.All)
            {
                //Check if all files for the champ exist
                bool loadingExists = File.Exists($"{location}/{champ.Alias}_loading.png");
                bool splashExists = File.Exists($"{location}/{champ.Alias}_splash.png");
                bool centeredSplashExists = File.Exists($"{location}/{champ.Alias}_centered_splash.png");
                bool squareExists = File.Exists($"{location}/{champ.Alias}_square.png");

                if (loadingExists && splashExists && centeredSplashExists && squareExists)
                {
                    $"{champ.Alias} asset cache verified".Debug("CDrag");
                    ExtendChampionLocal(champ, StringVersion.LeagueVersion);
                    CommunityDragonEventHandler.FileDownloadComplete?.Invoke(null, new FileLoadProgressEventArgs(champ.Alias, "Verified", IncrementDownloaded(4), _toDownload));
                    continue;
                }


                $"{champ.Alias} not downloaded. Retrieving from CommunityDragon".Debug("CDrag");
                //Get champ data if not all files exist
                await ExtendChampion(champ, _cfg!.Patch);
                count++;

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

                ExtendChampionLocal(champ, StringVersion.LeagueVersion);

                CommunityDragonEventHandler.FileDownloadComplete?.Invoke(null, new FileLoadProgressEventArgs(champ.Alias, "Verified", IncrementDownloaded(4), _toDownload));
            };



            $"Verified all champion assets. {count} downloaded".Info("CDrag");
        }


        private static void DownloadMissingItemCache(string location, ConcurrentBag<string[]> failedDownloads)
        {
            "Verifying Item Assets".Info("CDrag");
            foreach (Item item in Item.All)
            {
                if (!File.Exists($"{location}/{item.ID}.png"))
                {
                    $"{item.ID} not downloaded. Retrieving from CommunityDragon".Debug("CDrag");
                    DownloadAsset($"{_cfg!.CDragonRaw}/{_cfg.Patch}/plugins/rcp-be-lol-game-data/{_cfg.Region}/default/assets/items/icons2d/{item.IconPath.Split("/")[^1].ToLower()}", $"{location}/{item.ID}.png", $"{item.ID}.png", failedDownloads);
                }
                $"{item.ID} asset cache verified".Debug("CDrag");
                ExtendItemLocal(item, StringVersion.LeagueVersion);
                CommunityDragonEventHandler.FileDownloadComplete?.Invoke(null, new FileLoadProgressEventArgs(item.Name, "Verified", IncrementDownloaded(), _toDownload));
            };
        }

        private static void DownloadMissingSummonerSpellCache(string location, ConcurrentBag<string[]> failedDownloads)
        {
            "Verifying Summoner Spell Assets".Info("CDrag");
            foreach (SummonerSpell spell in SummonerSpell.All)
            {
                if (!File.Exists($"{location}/{spell.ID}.png"))
                {
                    $"{spell.ID} not downloaded. Retrieving from CommunityDragon".Debug("CDrag");
                    DownloadAsset($"{_cfg!.CDragonRaw}/{_cfg.Patch}/plugins/rcp-be-lol-game-data/{_cfg.Region}/default/data/spells/icons2d/{spell.IconPath.Split("/")[^1].ToLower()}", $"{location}/{spell.ID}.png", $"{spell.ID}.png", failedDownloads);
                }
                $"{spell.ID} asset cache verified".Debug("CDrag");
                ExtendSummonerLocal(spell, StringVersion.LeagueVersion);
                CommunityDragonEventHandler.FileDownloadComplete?.Invoke(null, new FileLoadProgressEventArgs(spell.Name, "Verified", IncrementDownloaded(), _toDownload));
            };
        }

        private static void DownloadAsset(string remote, string local, string fileName, ConcurrentBag<string[]> failedDownloads)
        {
            _ = IncrementToDownload();
            $"Downloading {fileName} from {remote}".Debug("CDrag");
            Task t = Task.Run(async () =>
            {
                System.Net.HttpStatusCode res = await FileDownloader.DownloadAsync(remote, local);
                if (res == System.Net.HttpStatusCode.OK)
                {
                    $"{fileName} downloaded".Debug("CDrag");
                }
                else
                {
                    failedDownloads.Add(new string[] { remote, fileName });
                    $"Download {fileName} from {remote} to {local} failed: {res}".Debug("CDrag");
                }

                CommunityDragonEventHandler.FileDownloadComplete?.Invoke(null, new FileLoadProgressEventArgs(fileName, "Downloaded", IncrementDownloaded(), _toDownload));

                if (_downloaded >= _toDownload)
                {
                    _ = _downloadComplete!.TrySetResult(true);
                }
            });
        }

        #region ObjectExtension
        public static async Task ExtendChampion(Champion champion, string version)
        {
            using JsonDocument response = JsonDocument.Parse(await RestRequester.GetRaw($"{_cfg!.CDragonRaw}{version}/plugins/rcp-be-lol-game-data/{_cfg.Region}/default/v1/champions/{champion.ID}.json"), new JsonDocumentOptions { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip });
            JsonElement root = response.RootElement;
            JsonElement defaultSkin = root.GetProperty("skins").EnumerateArray().Single(skin => $"{skin.GetProperty("id").GetInt32()}" == $"{champion.ID}000");

            champion.SplashImg = $"{_cfg.CDragonRaw}{version}/plugins/rcp-be-lol-game-data/{_cfg.Region}/default/v1/champion-splashes/uncentered/{champion.ID}/{(defaultSkin.GetProperty("uncenteredSplashPath").GetString() ?? "").Split("/")[^1]}";
            champion.SplashCenteredImg = $"{_cfg.CDragonRaw}{version}/plugins/rcp-be-lol-game-data/{_cfg.Region}/default/v1/champion-splashes/{champion.ID}/{(defaultSkin.GetProperty("splashPath").GetString() ?? "").Split("/")[^1]}";
            champion.SquareImg = $"{_cfg.CDragonRaw}{version}/plugins/rcp-be-lol-game-data/{_cfg.Region}/default/v1/champion-icons/{(root.GetProperty("squarePortraitPath").GetString() ?? "").Split("/")[^1]}";
            champion.LoadingImg = $"{_cfg.CDragonRaw}{version}/plugins/rcp-be-lol-game-data/{_cfg.Region}/default/assets/characters/{champion.Alias.ToLower()}/skins/base/{(defaultSkin.GetProperty("loadScreenPath").GetString() ?? "").Split("/")[^1].ToLower()}";
        }

        public static void ExtendChampionLocal(Champion champion, StringVersion version)
        {
            string championPath = $"cache/{version.ToString(2)}.1/champion";
            champion.SplashImg = $"{championPath}/{champion.Alias}_splash.jpg";
            champion.SplashCenteredImg = $"{championPath}/{champion.Alias}_centered_splash.png";
            champion.SquareImg = $"{championPath}/{champion.Alias}_square.png";
            champion.LoadingImg = $"{championPath}/{champion.Alias}_loading.png";
        }

        public static void ExtendSummonerLocal(SummonerSpell summoner, StringVersion version)
        {
            summoner.IconPath = $"cache/{version.ToString(2)}.1/spell/{summoner.ID}.png";
        }

        public static void ExtendItemLocal(Item item, StringVersion version)
        {
            item.IconPath = $"cache/{version.ToString(2)}.1/item/{item.ID}/.png";
        }

        #endregion
    }
}
