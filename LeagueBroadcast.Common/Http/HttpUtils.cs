using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LeagueBroadcast.Common.Http
{
    public static class HttpUtils
    {
        public static async Task<string> GetTextAsync(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Method = "GET";
            request.Timeout = 2000;
            try
            {
                Log.Verbose($"Downloading {uri}");
                using HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    Log.Warn($"Could not download {uri}: {response.StatusCode}");
                    return "";
                }
                using Stream stream = response.GetResponseStream();
                using StreamReader reader = new StreamReader(stream);
                Log.Verbose($"{uri} downloaded");
                return await reader.ReadToEndAsync();
            }
            catch (Exception e)
            {
                Log.Warn($"Could not download {uri}: {e.Message}");
                return "";
            }

        }
    }
}
