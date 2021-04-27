using LeagueBroadcast.Common;
using LeagueBroadcast.Common.Utils;
using LeagueBroadcast.MVVM.ViewModel;
using LeagueBroadcast.Update;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LeagueBroadcast.Common.Controllers
{
    class AppUpdateController
    {
        static TaskCompletionSource<bool> UpdateInput = null;
        public static async Task<bool> Update(StartupViewModel ctx)
        {
            var config = ConfigController.Component.App;
            if(!config.CheckForUpdates)
            {
                Log.Info("Update Check disabled");
                return false;
            }
            Log.Info("Checking for Updates");
            ctx.Status = "Checking for Updates";

            var latestRelease = await GitHubRemoteEndpoint.Instance.GetLatestReleaseAsync(config.UpdateRepositoryName);

            if (latestRelease == null)
            {
                return false;
            }

            if (!StringVersion.TryParse(latestRelease.Version, out var latestReleaseVersion))
            {
                return false;
            }

            Log.Info($"Latest release version {latestRelease.Version} vs. current version {config.Version}.");
            if (latestReleaseVersion <= config.Version)
            {
                return false;
            }

            if (!TryGetReleaseDownloadUrl(latestRelease, out var releaseDownloadUrl))
            {
                return false;
            }

            UpdateInput = new TaskCompletionSource<bool>();

            ctx.Status = "Update Found";
            ctx.UpdateText = $"An Update Is Available: v{latestReleaseVersion}";
            ctx.ShowUpdateDialog = true;

            ctx.Update += (s, e) => {
                Log.Info($"Updating LeagueBroadcastHub to v{latestReleaseVersion}.");
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
                                                 .Append($"cd /d \"{Environment.CurrentDirectory}\" && ")
                                                 .Append("start GoldDiff.exe\"");

                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
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
                Log.Info("Update skipped");
                UpdateInput.TrySetResult(false);
            };

            return await UpdateInput.Task;
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

                if (!asset.Name.Equals($"LB-{latestRelease.Version}.zip", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                url = asset.DownloadUrl;
                return true;
            }

            return false;
        }
    }
}
