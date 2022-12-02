using System;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using LeagueBroadcast.Utils.Log;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.IO;

namespace Common.Http
{
    //adapted from https://github.com/Johannes-Schneider/GoldDiff/blob/master/GoldDiff.Shared/Http/RestRequester.cs
    public class RestRequester : IDisposable
    {
        public static TimeSpan DefaultRequestTimeout { get; } = TimeSpan.FromMilliseconds(2000);

        private HttpClient Client { get; }

        private static RestRequester? _instance;
        private static RestRequester Instance => GetInstance();

        private static RestRequester GetInstance()
        {
            _instance ??= new RestRequester(TimeSpan.FromMilliseconds(2000), null);
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

        public static async Task<TResultType?> GetAsync<TResultType>(string url, ICollection<JsonConverter>? converters = null)
        {
            try
            {
                var response = await Instance.Client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    $"Request to {url} ({nameof(TResultType)} = {typeof(TResultType).Name}) returned status code {response.StatusCode}.".Warn();
                    return default;
                }
                var serializationOptions = new JsonSerializerOptions();

                if (converters != null)
                {
                    foreach (var converter in converters)
                    {
                        serializationOptions.Converters.Add(converter);
                    }
                }

                var json = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<TResultType>(json, serializationOptions)!;
            }
            catch (TaskCanceledException e)
            {
                $"Request to {url} caused a {nameof(TaskCanceledException)}. This is usually an indicator for a timeout.".Warn();
                $"{e}".Debug();
                return default;
            }
        }


        public static async Task<Stream?> GetStreamAsync(string url)
        {
            HttpResponseMessage? response = await Instance.Client.GetAsync(url).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            int totalData = (int)response.Content.Headers.ContentLength.GetValueOrDefault(-1);

            if(totalData > 0)
            {
                return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            }

            return default;
        }

        public static async Task<string> GetRaw(string url)
        {
            try
            {
                HttpResponseMessage? response = await Instance.Client.GetAsync(url).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    $"Request to {url} returned status code {response.StatusCode}.".Warn();
                    return default!;
                }
                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                $"Request to {url} caused a {nameof(TaskCanceledException)}. This is usually an indicator for a timeout.".Warn();
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
