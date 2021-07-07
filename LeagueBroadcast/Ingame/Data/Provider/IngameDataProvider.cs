using LeagueBroadcast.Common;
using LeagueBroadcast.Ingame.Data.RIOT;
using LeagueBroadcast.Ingame.Events;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace LeagueBroadcast.Ingame.Data.Provider
{
    class IngameDataProvider
    {
        public static HttpClient webClient;

        public IngameDataProvider()
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
            webClient.DefaultRequestHeaders.Add("User-Agent", "LeagueBroadcast");
            webClient.Timeout = TimeSpan.FromSeconds(0.25);
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

        public async Task<List<RiotEvent>> GetEventData()
        {
            HttpResponseMessage response = await webClient.GetAsync("https://127.0.0.1:2999/liveclientdata/eventdata");
            if (!response.IsSuccessStatusCode)
            {
                //Game Not Running
                return null;
            }

            var result = await response.Content.ReadAsStringAsync();

            var events = JsonConvert.DeserializeObject<RiotEventList>(result).Events;
            return events;
        }

        public async Task<bool> IsSpectatorGame(int tries = 0)
        {
            if(tries == 10)
            {
                Log.Warn("Could not determine if active game is spectator game. Defaulting to no to protect against usage in live games");
                return false;
            }
            try
            {
                Log.Verbose("Checking spectate endpoint to determine game type");
                HttpResponseMessage response = await webClient.GetAsync("https://127.0.0.1:2999/liveclientdata/activeplayername");
                string res = await response.Content.ReadAsStringAsync();
                if (res.Trim() == "\"\"")
                {
                    Log.Verbose("Found spectate game");
                    return true;
                }
                Log.Verbose("Found live game");
                return false;
            } catch (Exception e)
            {
                Log.Verbose($"Spectate endpoint connection error: {e.Message} \n Attempting again");
                return await IsSpectatorGame(tries++);
            }
            
        }
    }
}
