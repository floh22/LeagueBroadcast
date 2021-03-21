using LeagueBroadcastHub.Data.Containers;
using LeagueBroadcastHub.Data.Containers.Objectives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeagueBroadcastHub.Data
{
    class OCRDataProvider
    {

        public async Task<List<Objective>> GetObjectiveData()
        {
            try
            {
                var response = LeagueDataProvider.webClient.GetAsync("http://localhost:3002/api/objectives").Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<IEnumerable<Objective>>(result).ToList();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"{(int)response.StatusCode} ({response.ReasonPhrase})");
                    return null;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return null;
            }
        }

        public async Task<IEnumerable<OCRTeam>> GetTeamData()
        {
            try
            {
                var response = LeagueDataProvider.webClient.GetAsync("http://localhost:3002/api/teams").Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<IEnumerable<OCRTeam>>(result);
                }
                else
                {
                    //System.Diagnostics.Debug.WriteLine($"{(int)response.StatusCode} ({response.ReasonPhrase})");
                    return null;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return null;
            }
        }
    }
}
