using LeagueBroadcast.Common;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace LeagueBroadcast.Update.Http
{
    //https://github.com/Johannes-Schneider/GoldDiff/blob/master/GoldDiff.Shared/Http/RestRequester.cs
    public class RestRequester : IDisposable
    {
        public static TimeSpan DefaultRequestTimeout { get; } = TimeSpan.FromMilliseconds(500);

        private HttpClient Client { get; }

        public RestRequester(TimeSpan requestTimeout, HttpClientHandler? clientHandler = null)
        {
            Client = new HttpClient(clientHandler ?? new HttpClientHandler())
            {
                Timeout = requestTimeout,
                DefaultRequestHeaders =
                         {
                             Accept = {MediaTypeWithQualityHeaderValue.Parse("application/json")},
                             UserAgent = {ProductInfoHeaderValue.Parse("request")},
                         },
            };
        }

        public async Task<TResultType> GetAsync<TResultType>(string url)
        {
            try
            {
                var response = await Client.GetAsync(url).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    Log.Warn($"Request to {url} ({nameof(TResultType)} = {typeof(TResultType).Name}) returned status code {response.StatusCode}.");
                    return default!;
                }
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<TResultType>(json);
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
