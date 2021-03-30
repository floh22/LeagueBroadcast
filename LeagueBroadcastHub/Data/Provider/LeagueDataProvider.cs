using LeagueBroadcastHub.Data.Game.RiotContainers;
using LeagueBroadcastHub.Events.Game.RiotEvents;
using LeagueBroadcastHub.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace LeagueBroadcastHub.Data.Provider
{
    class LeagueDataProvider
    {
        public static HttpClient webClient;
        public void Init()
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
            webClient.Timeout = TimeSpan.FromSeconds(0.5);
        }

        public async Task<GameMetaData> GetGameData()
        {
            try
            {
                var response = webClient.GetAsync("https://127.0.0.1:2999/liveclientdata/gamestats").Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<GameMetaData>(result);
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

        public async Task<List<Player>> GetPlayerData()
        {
            HttpResponseMessage response = await webClient.GetAsync("https://127.0.0.1:2999/liveclientdata/playerlist");
            if (!response.IsSuccessStatusCode)
            {
                //Game Not Running
                return null;
            }
            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<Player>>(result).ToList();
        }

        public async Task<List<DirtyEvent>> GetEventData()
        {
            HttpResponseMessage response = await webClient.GetAsync("https://127.0.0.1:2999/liveclientdata/eventdata");
            if (!response.IsSuccessStatusCode)
            {
                //Game Not Running
                return null;
            }

            var result = await response.Content.ReadAsStringAsync();

            var events = JsonConvert.DeserializeObject<DirtyEventList>(result).Events;
            return events;
        }
    }
}
