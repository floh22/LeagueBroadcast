using LeagueBroadcastHub.Events;
using LeagueBroadcastHub.Events.RiotEvents;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace LeagueBroadcastHub.Data
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
                    //System.Diagnostics.Debug.WriteLine($"{(int)response.StatusCode} ({response.ReasonPhrase})");
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

            /*
             * 
             * Move to directly using the event instead of trying to manually clean it
            System.Diagnostics.Debug.WriteLine("Found " + riotEvents.Count + " Game Events");

            List<RiotEvent> riotEventsCleaned = new List<RiotEvent>();

            riotEvents.ForEach((e) =>
            {
                string name = e.EventName;
                switch (name)
                {
                    case "GameStart":
                        riotEventsCleaned.Add(new GameStart(e));
                        break;
                    case "GameEnd":
                        riotEventsCleaned.Add(new GameEnd(e));
                        break;
                    case "MinionsSpawning":
                        riotEventsCleaned.Add(new MinionsSpawning(e));
                        break;
                    case "FirstBrick":
                        riotEventsCleaned.Add(new FirstBrick(e));
                        break;
                    case "FirstBlood":
                        riotEventsCleaned.Add(new FirstBlood(e));
                        break;
                    case "TurretKilled":
                        riotEventsCleaned.Add(new TurretKilledEvent(e));
                        break;
                    case "InhibKilled":
                        riotEventsCleaned.Add(new InhibKilled(e));
                        break;
                    case "InhibRespawned":
                        riotEventsCleaned.Add(new InhibRespawnedEvent(e));
                        break;
                    case "InhibRespawningSoon":
                        riotEventsCleaned.Add(new InhibRespawningSoonEvent(e));
                        break;
                    case "ChampionKill":
                        riotEventsCleaned.Add(new ChampionKill(e));
                        break;
                    case "Multikill":
                        riotEventsCleaned.Add(new Multikill(e));
                        break;
                    case "Ace":
                        riotEventsCleaned.Add(new Ace(e));
                        break;
                    default:
                        Console.WriteLine("Something went wrong determining Riot Event Type " + e.EventName);
                        Console.WriteLine("Event received: " + JsonConvert.SerializeObject(e));
                        break;
                }
            });

            return riotEventsCleaned;
            */
        }
    }
}
