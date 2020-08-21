using System;
using System.Net.Http;
using System.Threading.Tasks;
using AireLogic.ApiSeed.ResponseModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AireLogic.ArtistData.Services
{
    public class ApiSeedsService : ILyricService
    {
        private readonly ILogger<ApiSeedsService> _logger;
        private readonly IConfiguration _configuration;

        public ApiSeedsService(ILogger<ApiSeedsService> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> GetLyrics(string artist, string track)
        {
            // remove text at the end of the track name which is not relevant
            var trackSuffixIndex = track.IndexOfAny(new char[] { '-', '(' });
            if (trackSuffixIndex > 0)
                track = track.Substring(0, trackSuffixIndex);

            var baseUrl = _configuration.GetValue<string>("ApiSeeds:BaseUrl");
            var apiKey = _configuration.GetValue<string>("ApiSeeds:ApiKey");
            var apiSeedClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
            var response = await apiSeedClient.GetAsync($"api/music/lyric/{artist}/{track}?apikey={apiKey}");

            if (response.IsSuccessStatusCode)
            {
                var lyricContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var lyricRoot = JsonConvert.DeserializeObject<RootNode>(lyricContent);
                return lyricRoot.Result.Track.Text;
            }
            else
            {
                _logger.LogWarning("Lyrics could not be loaded: {0}", response.ReasonPhrase);
            }
            return string.Empty;
        }
    }
}