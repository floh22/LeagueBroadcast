using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LeagueBroadcast.Update.Http
{
    //Adapted from https://github.com/Johannes-Schneider/GoldDiff/blob/master/GoldDiff.Shared/Http/FileDownloader.cs
    public class FileDownloader
    {
        public static async Task<HttpStatusCode> DownloadAsync(string? remoteUrl, string? filePath, IProgress<float>? progress = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(remoteUrl))
            {
                throw new ArgumentNullException(nameof(remoteUrl));
            }

            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            string? fileDirectory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(fileDirectory))
            {
                _ = Directory.CreateDirectory(fileDirectory!);
            }

            if (File.Exists(filePath))
            {
                File.Delete(filePath!);
            }


            using HttpClient httpClient = new();
            httpClient.Timeout = TimeSpan.FromMinutes(5);
            using FileStream file = new(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            return await httpClient.DownloadAsync(remoteUrl, file, progress, cancellationToken);
        }
    }
}
