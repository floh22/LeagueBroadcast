using LeagueBroadcastHub.Data.Replay;
using LeagueBroadcastHub.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LeagueBroadcastHub.Data.Provider
{
    class ReplayDataProvider
    {
        private static HttpClient webClient;

        public ReplayDataProvider()
        {
            Init();
        }

        private void Init()
        {
            var handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return true;
                }
            };

            webClient = new HttpClient(handler);
            webClient.DefaultRequestHeaders.ExpectContinue = true;
            webClient.DefaultRequestHeaders.Add("User-Agent", "LeagueBroadcastHub");
            webClient.Timeout = TimeSpan.FromSeconds(0.25);
        }

        public static async Task<Replay.Game> GETGameAsync()
        {
            try
            {
                var response = webClient.GetAsync("https://127.0.0.1:2999/replay/game").Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Replay.Game>(result);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<Playback> GETPlaybackAsync()
        {
            try
            {
                var response = webClient.GetAsync("https://127.0.0.1:2999/replay/playback").Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Playback>(result);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<bool> POSTPlaybackAsync(Playback playback)
        {
            var content = new StringContent(JsonConvert.SerializeObject(playback), Encoding.UTF8, "application/json");
            var result = await webClient.PostAsync("https://127.0.0.1:2999/replay/playback", content );
            return result.IsSuccessStatusCode;

        }

        public static async Task<Render> GETRenderAsync()
        {
            try
            {
                var response = webClient.GetAsync("https://127.0.0.1:2999/replay/render").Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Render>(result);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<bool> POSTRenderAsync(Render render)
        {
            var c = JsonConvert.SerializeObject(render);
            //Logging.Verbose(c);
            var content = new StringContent(c, Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await webClient.PostAsync("https://127.0.0.1:2999/replay/render", content);
            Logging.Verbose("Render Post Result: " + result.StatusCode.ToString());
            return result.IsSuccessStatusCode;
        }

    }
}
