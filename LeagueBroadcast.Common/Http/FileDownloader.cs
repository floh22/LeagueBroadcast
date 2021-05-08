using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace LeagueBroadcast.Update.Http
{
    //https://github.com/Johannes-Schneider/GoldDiff/blob/master/GoldDiff.Shared/Http/FileDownloader.cs
    public class FileDownloader
    {
#nullable enable
        public event EventHandler<DownloadProgressEventArguments>? DownloadProgressChanged;

        public async Task DownloadAsync(string? remoteUrl, string? filePath)
        {
            if (string.IsNullOrEmpty(remoteUrl))
            {
                throw new ArgumentNullException(nameof(remoteUrl));
            }

            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            var fileDirectory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(fileDirectory))
            {
                Directory.CreateDirectory(fileDirectory!);
            }

            if (File.Exists(filePath))
            {
                File.Delete(filePath!);
            }

            using var webClient = new WebClient();
            var downloadStartTime = DateTime.Now;
            webClient.DownloadProgressChanged += (_, args) =>
            {
                if (DownloadProgressChanged == null)
                {
                    return;
                }

                var elapsedTime = DateTime.Now - downloadStartTime;
                var progress = args.BytesReceived / (double)args.TotalBytesToReceive;
                var bytesPerSecond = args.BytesReceived / elapsedTime.TotalSeconds;
                var estimatedRemainingTime = TimeSpan.FromSeconds((args.TotalBytesToReceive - args.BytesReceived) / bytesPerSecond);

                DownloadProgressChanged.Invoke(this, new DownloadProgressEventArguments(progress,
                                                                                        bytesPerSecond / (1024.0d * 1024.0d),
                                                                                        estimatedRemainingTime));
            };
            await webClient.DownloadFileTaskAsync(remoteUrl!, filePath!);
        }
    }
#nullable disable
}
