using LeagueBroadcast.Common;
using LeagueBroadcast.Common.Utils;
using LeagueBroadcast.MVVM.ViewModel;
using LeagueBroadcast.Update;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueBroadcast.Common.Controllers
{
    //https://github.com/Johannes-Schneider/GoldDiff/blob/master/GoldDiff/App.xaml.cs
    class AppUpdateController
    {
        static TaskCompletionSource<bool> UpdateInput = null;
        public static async Task<bool> Update(StartupViewModel ctx)
        {
            var config = ConfigController.Component.App;
            if(!config.CheckForUpdates)
            {
                Log.Info("[Update] Update Check disabled");
                return false;
            }
            Log.Info("[Update] Checking for Updates");
            ctx.Status = "Checking for Updates";

            var latestRelease = await GitHubRemoteEndpoint.Instance.GetLatestReleaseAsync(config.UpdateRepositoryName);

            if (latestRelease == null)
            {
                Log.Warn("[Update] Could not find latest release");
                return false;
            }

            if (!StringVersion.TryParse(GetVersionNumber(latestRelease.Version), out var latestReleaseVersion))
            {
                Log.Warn($"[Update] Could not parse version of latest release {latestRelease.Version}");
                return false;
            }

            Log.Info($"[Update] Latest release version {latestRelease.Version} vs. current version {config.Version}");
            if (latestReleaseVersion <= config.Version)
            {
                Log.Info("[Update] Local version up to date");
                return false;
            }

            if (!TryGetReleaseDownloadUrl(latestRelease, out var releaseDownloadUrl))
            {
                Log.Warn("[Update] Could not find latest release location");
                return false;
            }

            UpdateInput = new TaskCompletionSource<bool>();

            ctx.Status = "Update Found";
            ctx.UpdateText = $"An Update Is Available: v{latestReleaseVersion}";
            ctx.ShowUpdateDialog = true;
            Log.Info("[Update] Update found");

            ctx.Update += (s, e) => {
                Log.Info($"[Update] Updating LeagueBroadcast to v{latestReleaseVersion}.");
                ctx.Status = "Downloading Update";
                var temporaryPath = Environment.CurrentDirectory;
                var latestReleaseDownloadFile = Path.Combine(temporaryPath, Path.GetTempFileName());
                var unpackedDirectory = Path.Combine(temporaryPath, $"LeagueBroadcast {latestRelease.Version}");

                var command = new StringBuilder().Append("/C \"")
                                                 .Append($"cd \"{temporaryPath}\" && ")
                                                 .Append($"curl -s -L \"{releaseDownloadUrl}\" > \"{latestReleaseDownloadFile}\" && ")
                                                 .Append($"tar -xf \"{latestReleaseDownloadFile}\" > NUL && ")
                                                 .Append($"xcopy \"{unpackedDirectory}\\*.*\" \"{Environment.CurrentDirectory}\" /y > NUL && ")
                                                 .Append($"del \"{latestReleaseDownloadFile}\" > NUL && ")
                                                 .Append($"rmdir /q /s \"{unpackedDirectory}\" > NUL && ")
                                                 .Append($"cd /d \"%~dp0\" && ")
                                                 .Append("start LeagueBroadcast.exe\"");

                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        WorkingDirectory = Environment.CurrentDirectory,
                        Arguments = command.ToString(),
                    });
                    UpdateInput.TrySetResult(true);
                }
                catch
                {
                    // TODO: implement error handling
                }

                UpdateInput.TrySetResult(false);
            };

            ctx.SkipUpdate += (s, e) => {
                Log.Info("[Update] Update skipped");
                UpdateInput.TrySetResult(false);
            };
            Log.Info("[Update] Asking user about application update. Halting startup");
            Log.WriteToFileAndPause();
            bool res = await UpdateInput.Task;
            ctx.ShowUpdateDialog = false;
            Log.Resume();
            return res;
        }

        private static bool TryGetReleaseDownloadUrl(GitHubReleaseInfo latestRelease, out string url)
        {
            url = string.Empty;
            foreach (var asset in latestRelease.Assets)
            {
                if (!asset.ContentType.Equals("application/x-zip-compressed", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                if (!asset.Name.Equals($"LeagueBroadcast-{GetVersionNumber(latestRelease.Version)}.zip", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                url = asset.DownloadUrl;
                return true;
            }

            return false;
        }

        private static string GetVersionNumber(string input)
        {
            return new string(input.Where(c => char.IsDigit(c) || c == '.').ToArray());
        }
    }
}
