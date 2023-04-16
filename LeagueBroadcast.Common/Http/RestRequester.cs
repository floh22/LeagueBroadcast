using System;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using LeagueBroadcast.Common;
using Newtonsoft.Json;

namespace LeagueBroadcast.Update.Http
{
    //https://github.com/Johannes-Schneider/GoldDiff/blob/master/GoldDiff.Shared/Http/RestRequester.cs
    public class RestRequester : IDisposable
    {
        public static TimeSpan DefaultRequestTimeout { get; } = TimeSpan.FromMilliseconds(2000);

        private HttpClient Client { get; }

        private static RestRequester? _instance;
        private static RestRequester Instance => GetInstance();

        private static RestRequester GetInstance()
        {
            if (_instance == null)
            {
                _instance = new RestRequester(TimeSpan.FromMilliseconds(2000), null);
            }
            return _instance;
        }

        private RestRequester(TimeSpan requestTimeout, HttpClientHandler? clientHandler = null)
        {

            Client = new HttpClient(clientHandler ?? new HttpClientHandler())
            {
                Timeout = requestTimeout,
            };

            Client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
            Client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("LeagueBroadcast", "2.0"));
        }

        public static async Task<TResultType?> GetAsync<TResultType>(string url)
        {
            try
            {
                var response = await Instance.Client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    Log.Warn($"Request to {url} ({nameof(TResultType)} = {typeof(TResultType).Name}) returned status code {response.StatusCode}.");
                    return default;
                }
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject< TResultType>(json)!;
            }
            catch (TaskCanceledException)
            {
                Log.Warn($"Request to {url} caused a {nameof(TaskCanceledException)}. This is usually an indicator for a timeout.");
                return default;
            }
        }

        public static async Task<string> GetRaw(string url)
        {
            try
            {
                HttpResponseMessage? response = await Instance.Client.GetAsync(url).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    Log.Warn($"Request to {url} returned status code {response.StatusCode}.");
                    throw new HttpRequestException(response.StatusCode + "");
                }
                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                Log.Warn($"Request to {url} caused a {nameof(TaskCanceledException)}. This is usually an indicator for a timeout.");
                return default!;
            }
        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~RestRequester()
        {
            Dispose(false);
        }

        protected bool _isDisposed;

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            if (disposing)
            {
                Client.Dispose();
            }
        }

        #endregion
    }
}
