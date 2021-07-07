using LeagueBroadcast.Ingame.Data.Provider;
using LeagueBroadcast.Ingame.Data.Replay;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LeagueBroadcast.Common.Controllers
{
    class ReplayAPIController
    {

        private HttpClient webClient;

        private string result;

        public ReplayAPIController()
        {
            AppStateController.GameLoad += OnGameLoad;

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
            webClient.DefaultRequestHeaders.Add("User-Agent", "LeagueBroadcast");
            webClient.Timeout = TimeSpan.FromSeconds(0.25);
        }

        public async void OnGameLoad(object sender, EventArgs e)
        {
            await Task.Delay(1000);
            Log.Info("Init UI");
            result = await GetRequestContent("https://127.0.0.1:2999/replay/render");

            if (result == "")
            {
                Log.Warn("Received invalid response from replay API. Is it enabled?");
                return;
            }

            if (ConfigController.Component.Replay.UseAutoInitUI)
            {
                GameInputController.InitUI();
            }

            if (ConfigController.Component.Ingame.UseCustomScoreboard && result.Substring(result.IndexOf("interfaceScoreboard") + "interfaceScoreboard".Length + 3, 6).Contains("false"))
            {
                Log.Verbose("Disabling ingame score display");
                _ = PostString("https://127.0.0.1:2999/replay/render", DisableScore());
            }
        }

        private string EnableScore()
        {
            int toChange = result.IndexOf("interfaceScore");
            result = result.Remove(toChange + "interfaceScore".Length + 3, 5).Insert(toChange + "interfaceScore".Length + 3, "true");
            return result;
        }

        private string DisableScore()
        {
            int toChange = result.IndexOf("interfaceScore");
            result = result.Remove(toChange + "interfaceScore".Length + 3, 4).Insert(toChange + "interfaceScore".Length + 3, "false");
            return result;
        }


        public async Task<string> GetRequestContent(string url)
        {
            var response = webClient.GetAsync(url).Result;

            if (!response.IsSuccessStatusCode)
                return "";

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<HttpResponseMessage> PostString(string url, string input)
        {
            var content = new StringContent(input, Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return await webClient.PostAsync(url, content);
        }
    }
}
