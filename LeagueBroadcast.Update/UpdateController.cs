using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LeagueBroadcast.Utils;
using LeagueBroadcast.Update.GitHub;
using LeagueBroadcast.Utils.Log;
using LeagueBroadcast.Common.Config;

namespace LeagueBroadcast.Update
{
    public class UpdateController
    {

        public static event EventHandler<StringVersion>? UpdateFound, UpdateSkipped;
        public static event EventHandler? UpdateConfirmed, UpdateCanceled, UpdateDownloaded, UpdateFailed;

        private static TaskCompletionSource<bool>? UpdateInput;
        private static Process? UpdateProcess;

        #region EventInvoke
        public static void OnUpdateConfirmed()
        {
            UpdateConfirmed?.Invoke(null, EventArgs.Empty);
        }

        public static void OnUpdateFound(StringVersion version)
        {
            UpdateFound?.Invoke(null, version);
        }

        public static void OnUpdateCanceled()
        {
            UpdateCanceled?.Invoke(null, EventArgs.Empty);
        }

        public static void OnUpdateSkipped(StringVersion version)
        {
            UpdateSkipped?.Invoke(null, version);
        }

        public static void OnUpdateDownloaded()
        {
            UpdateDownloaded?.Invoke(null, EventArgs.Empty);
        }

        public static void OnUpdateFailed()
        {
            UpdateFailed?.Invoke(null, EventArgs.Empty);
        }
        #endregion

        public static async Task<bool> CheckForUpdate(string updateFileName, StringVersion skippedUpdate)
        {
            AppConfig? config = ConfigController.Get<AppConfig>();
            if (!config.CheckForUpdates)
            {
                "[Update] Update Check disabled".Info();
                return false;
            }
            "[Update] Checking for Updates".Info();
            // "Checking for Updates".UpdateLoadStatus();

            return false;

            GitHubReleaseInfo? latestRelease = await GitHubRemoteEndpoint.GetLatestReleaseAsync(config.UpdateRepositoryName);
            if (latestRelease == null)
            {
                "[Update] Could not find latest release".Error();
                return false;
            }

            if (!GetVersionNumberAndFileLocation(updateFileName, latestRelease, out StringVersion latestReleaseVersion, out string releaseDownloadUrl))
            {
                $"[Update] Could not parse version of latest release {latestRelease.Version}".Error();
                return false;
            }

            StringVersion appVersion = StringVersion.AppVersion;
            $"[Update] Latest release version {latestReleaseVersion} vs. current version {appVersion}".Info();
            if (latestReleaseVersion <= appVersion || latestReleaseVersion == null)
            {
                "[Update] Local version up to date".Info();
                //return false;
            }

            if (latestReleaseVersion <= skippedUpdate)
            {
                "[Update] Latest update skipped by user".Info();
                return false;
            }

            UpdateInput = new TaskCompletionSource<bool>();

            // "Update Found".UpdateLoadStatus();
            "[Update] Update found".Info();
            UpdateFound?.Invoke(null, latestReleaseVersion!);

            UpdateConfirmed += (s, e) => {
                $"[Update] Updating LeagueBroadcast to v{latestReleaseVersion}.".Info();
                // "Downloading Update".UpdateLoadStatus();
                string temporaryPath = Environment.CurrentDirectory;
                string latestReleaseDownloadFile = Path.Combine(temporaryPath, Path.GetTempFileName());
                string unpackedDirectory = Path.Combine(temporaryPath, $"LeagueBroadcast {latestRelease.Version}");

                StringBuilder command = new StringBuilder().Append("/C \"")
                                                 .Append($"cd \"{temporaryPath}\" && ")
                                                 .Append($"curl -s -L \"{releaseDownloadUrl}\" > \"{latestReleaseDownloadFile}\" && ")
                                                 .Append($"tar -xf \"{latestReleaseDownloadFile}\" > NUL && ")
                                                 .Append($"xcopy \"{unpackedDirectory}\\*.*\" \"{Directory.GetCurrentDirectory()}\" /y > NUL && ")
                                                 .Append($"del \"{latestReleaseDownloadFile}\" > NUL && ")
                                                 .Append($"rmdir /q /s \"{unpackedDirectory}\" > NUL");

                try
                {
                    _ = Task.Run(() =>
                    {
                        UpdateProcess = Process.Start(new ProcessStartInfo
                        {
                            FileName = "cmd.exe",
                            WorkingDirectory = Environment.CurrentDirectory,
                            Arguments = command.ToString(),
                        }) ?? throw new InvalidOperationException();
                        UpdateProcess.Exited += (s, e) => UpdateDownloaded?.Invoke(null, EventArgs.Empty);
                    });
                    _ = UpdateInput.TrySetResult(true);
                }
                catch
                {
                    UpdateFailed?.Invoke(null, EventArgs.Empty);
                }
                _ = UpdateInput.TrySetResult(false);
            };

            UpdateCanceled += (s, e) => {
                "[Update] Update ignored. Asking again next time".Info();
                _ = UpdateInput.TrySetResult(false);
            };

            UpdateSkipped += (s, e) =>
            {
                $"[Update] Update v{e} skipped. Will ask again next update".Info();
                _ = UpdateInput.TrySetResult(false);
            };

            //Wait for user to respond to update request
            "[Update] Asking user about application update. Waiting for reply".Info();
            return await UpdateInput.Task;
        }

        public static void RestartApplication()
        {
            ProcessStartInfo Info = new()
            {
                Arguments = "/C choice /C Y /N /D Y /T 1 & START \"\" \"" + Assembly.GetEntryAssembly()!.Location + "\"",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                FileName = "cmd.exe"
            };
            _ = Process.Start(Info);
            Process.GetCurrentProcess().Kill();
        }

        private static bool GetVersionNumberAndFileLocation(string fileName, GitHubReleaseInfo latestRelease, out StringVersion versionNumber, out string url)
        {
            url = string.Empty;
            foreach (GitHubReleaseAsset asset in latestRelease.Assets)
            {
                if (!asset.ContentType.Equals("application/x-zip-compressed", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!asset.Name.StartsWith($"{fileName}", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                url = asset.DownloadUrl;
                versionNumber = StringVersion.TryParse(GetVersionNumber(asset.Name), out StringVersion? fileVersion) ? fileVersion! : StringVersion.Zero;
                return true;
            }

            versionNumber = StringVersion.Zero;
            return false;
        }

        private static string GetVersionNumber(string input)
        {
            return new string(input.Where(c => char.IsDigit(c) || c == '.').ToArray());
        }
    }
}
