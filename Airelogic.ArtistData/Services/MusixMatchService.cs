using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AireLogic.MusixMatch.RespoonseModel;

namespace AireLogic.ArtistData.Services
{
    public class MusixMatchService : IMusicService
    {        
        const string musixMatchApiKey = "c1ffdab49fecac0576a99d199b81fe0f";

        public async Task<IEnumerable<string>> FindArtistTracks(string artistString)
        {
            Artist artist = null;

            var musixMatchClient = new HttpClient();
            musixMatchClient.BaseAddress = new Uri("https://api.musixmatch.com/ws/1.1/");
            var artistSearchResponse = await musixMatchClient.GetAsync($"artist.search?apikey={musixMatchApiKey}&q_artist={artistString}&page_size=5");
            
            if (artistSearchResponse.IsSuccessStatusCode)
            {
                var content = artistSearchResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var root = Newtonsoft.Json.JsonConvert.DeserializeObject<RootNode<ArtistListBody>>(content);
                Console.WriteLine("\nArtists");
                foreach (var a in root.Message.Body.artist_list.Select(al => al.Artist))
                {
                    Console.WriteLine($"{a.artist_id} {a.artist_name} {a.artist_rating} {a.artist_country}");
                }
                artist = root.Message.Body.artist_list.FirstOrDefault().Artist;
                artistString = artist.artist_name;

                var trackResponse = await musixMatchClient.GetAsync($"track.search?apikey={musixMatchApiKey}&f_artist_id={artist.artist_id}");

                if (trackResponse.IsSuccessStatusCode)
                {
                    var trackContent = trackResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    var trackRoot = Newtonsoft.Json.JsonConvert.DeserializeObject<RootNode<TrackListBody>>(trackContent);                    
                    return trackRoot.Message.Body.track_list.Select(t => t.track).Select(t => t.track_name);
                }
                else
                {
                    Console.WriteLine(trackResponse.ReasonPhrase);
                }
            }
            else
            {
                Console.WriteLine(artistSearchResponse.ReasonPhrase);
            }
            return null;
        }
    }
}