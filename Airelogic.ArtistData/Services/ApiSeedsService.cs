using System;
using System.Net.Http;
using System.Threading.Tasks;
using AireLogic.ApiSeed.ResponseModel;
using Newtonsoft.Json;

namespace AireLogic.ArtistData.Services
{
    public class ApiSeedsService : ILyricService
    {
        public async Task<string> GetLyrics(string artist, string track)
        {
            // remove text at the end of the track name which is not relevant
            var trackSuffixIndex = track.IndexOfAny(new char[] { '-', '(' });
            if (trackSuffixIndex > 0)
                track = track.Substring(0, trackSuffixIndex);

            var apiSeedClient = new HttpClient
            {
                BaseAddress = new Uri("https://orion.apiseeds.com/")
            };
            const string apiKey = "x3ALheLaELqwzOckiIUxMw2WS5DVbxZySuKpLgefJiECPLPTTYxA2heer8pcStx2";
            var response = await apiSeedClient.GetAsync($"api/music/lyric/{artist}/{track}?apikey={apiKey}");

            if (response.IsSuccessStatusCode)
            {
                var lyricContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var lyricRoot = JsonConvert.DeserializeObject<RootNode>(lyricContent);
                return lyricRoot.Result.Track.Text;
            }
            else
            {
                Console.WriteLine($"Lyrics could not be loaded: {response.ReasonPhrase}");
            }
            return string.Empty;
        }
    }
}