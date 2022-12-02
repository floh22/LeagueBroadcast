using Common.Http;
using LeagueBroadcast.Utils;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace LeagueBroadcast.Utils.Http
{
    //Adapted from https://github.com/Johannes-Schneider/GoldDiff/blob/master/GoldDiff.Shared/Http/FileDownloader.cs
    public class FileDownloader
    {
        public static TimeSpan DefaultRequestTimeout { get; } = TimeSpan.FromMinutes(5);

        private HttpClient Client { get; }

        private static FileDownloader? _instance;
        private static FileDownloader Instance => GetInstance();

        private static FileDownloader GetInstance()
        {
            _instance ??= new FileDownloader(TimeSpan.FromMinutes(5), null);
            return _instance;
        }

        private FileDownloader(TimeSpan requestTimeout, HttpClientHandler? clientHandler = null)
        {

            Client = new HttpClient(clientHandler ?? new HttpClientHandler())
            {
                Timeout = requestTimeout,
            };

            Client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("LeagueBroadcast", "2.0"));
        }



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


            using FileStream file = new (filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            return await Instance.Client.DownloadAsync(remoteUrl, file, progress, cancellationToken);
        }
    }
}
