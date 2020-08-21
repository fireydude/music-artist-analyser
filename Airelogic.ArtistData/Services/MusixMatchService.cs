using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AireLogic.MusixMatch.RespoonseModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AireLogic.ArtistData.Services
{
    public class MusixMatchService : IMusicService
    {
        private readonly ILogger<MusixMatchService> _logger;
        private readonly IConfiguration _configuration;

        public MusixMatchService(ILogger<MusixMatchService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<IEnumerable<string>> FindArtistTracks(string artistString)
        {
            Artist artist = null;

            var baseUrl = _configuration.GetValue<string>("MusixMatch:BaseUrl");
            var musixMatchApiKey = _configuration.GetValue<string>("MusixMatch:ApiKey");

            var musixMatchClient = new HttpClient();
            musixMatchClient.BaseAddress = new Uri(baseUrl);
            var artistSearchResponse = await musixMatchClient.GetAsync($"artist.search?apikey={musixMatchApiKey}&q_artist={artistString}&page_size=5");

            if (artistSearchResponse.IsSuccessStatusCode)
            {
                _logger.LogInformation("Searching Artists");
                var content = artistSearchResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var root = Newtonsoft.Json.JsonConvert.DeserializeObject<RootNode<ArtistListBody>>(content);
                foreach (var a in root.Message.Body.artist_list.Select(al => al.Artist))
                {
                    _logger.LogInformation("id:{artist_id} {artist_name} rating:{artist_rating} country:{artist_country}", a.artist_id, a.artist_name, a.artist_rating, a.artist_country);
                }
                artist = root.Message.Body.artist_list.FirstOrDefault().Artist;
                _logger.LogInformation("The first artist {a} will be used\n", artist.artist_name);
                artistString = artist.artist_name;

                _logger.LogInformation("Searching Tracks");
                var trackResponse = await musixMatchClient.GetAsync($"track.search?apikey={musixMatchApiKey}&f_artist_id={artist.artist_id}");

                if (trackResponse.IsSuccessStatusCode)
                {
                    var trackContent = trackResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    var trackRoot = Newtonsoft.Json.JsonConvert.DeserializeObject<RootNode<TrackListBody>>(trackContent);
                    return trackRoot.Message.Body.track_list.Select(t => t.track).Select(t => t.track_name);
                }
                else if (trackResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogWarning(trackResponse.ReasonPhrase);
                }
                else
                {
                    _logger.LogError(trackResponse.ReasonPhrase);
                }
            }
            else if (artistSearchResponse.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning(artistSearchResponse.ReasonPhrase);
            }
            else
            {
                _logger.LogWarning(artistSearchResponse.ReasonPhrase);
            }
            return null;
        }
    }
}