using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Airelogic.ArtistData.MusixMatch.RespoonseModel;
using ApiSeedModel = Airelogic.ArtistData.ApiSeed.ResponseModel;

namespace Airelogic.ArtistData
{
    class Program
    {
        const string musixMatchApiKey = "c1ffdab49fecac0576a99d199b81fe0f";


        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Please provide one argument, which will be the artist name");
                return;
            }
            var artistSearch = args[0];
            var allWordCounts = new List<int>();
            Artist artist = null;

            var musixMatchClient = new HttpClient();
            musixMatchClient.BaseAddress = new Uri("https://api.musixmatch.com/ws/1.1/");
            var artistSearchResponse = musixMatchClient.GetAsync($"artist.search?apikey={musixMatchApiKey}&q_artist={artistSearch}&page_size=5")
                .GetAwaiter()
                .GetResult();

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

                var trackResponse = musixMatchClient.GetAsync($"track.search?apikey={musixMatchApiKey}&f_artist_id={artist.artist_id}")
                    .GetAwaiter()
                    .GetResult();

                if (trackResponse.IsSuccessStatusCode)
                {
                    var trackContent = trackResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    var trackRoot = Newtonsoft.Json.JsonConvert.DeserializeObject<RootNode<TrackListBody>>(trackContent);
                    Console.WriteLine("\nTop 10 Tracks");
                    foreach (var t in trackRoot.Message.Body.track_list.Select(t => t.track))
                    {
                        Console.WriteLine($"{t.track_name} \n\t- {t.album_name}");
                        int wordCount;
                        if(LyricWordCount(artist.artist_name, t.track_name, out wordCount))
                        {
                            allWordCounts.Add(wordCount);
                            Console.WriteLine($"Word Count: {wordCount}\n");
                        }
                    }
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

            if (artist != null)
            {
                var averageWordCount = allWordCounts.Sum() / allWordCounts.Count();
                Console.WriteLine($"The average number of words for a song by {artist.artist_name} is {averageWordCount}");
            }
        }

        static bool LyricWordCount(string artist, string track, out int count)
        {
            count = 0;
            // remove text at the end of the track name which is not relevant
            var trackSuffixIndex = track.IndexOfAny(new char[] { '-', '(' });
            if (trackSuffixIndex > 0)
                track = track.Substring(0, trackSuffixIndex);

            var apiSeedClient = new HttpClient
            {
                BaseAddress = new Uri("https://orion.apiseeds.com/")
            };
            const string apiKey = "x3ALheLaELqwzOckiIUxMw2WS5DVbxZySuKpLgefJiECPLPTTYxA2heer8pcStx2";
            var response = apiSeedClient.GetAsync($"api/music/lyric/{artist}/{track}?apikey={apiKey}")
                .GetAwaiter()
                .GetResult();

            if (response.IsSuccessStatusCode)
            {
                var lyricContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var lyricRoot = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiSeedModel.RootNode>(lyricContent);
                count = CountWords(lyricRoot.Result.Track.Text);
            }
            else
            {
                Console.WriteLine($"Word count failed: {response.ReasonPhrase}");
            }
            return response.IsSuccessStatusCode;
        }

        static int CountWords(string songText)
        {
            var count = 0;
            foreach (var line in songText.Split('\n'))
            {
                if (line.StartsWith('['))
                    continue;
                count += line.Trim().Split(' ').Count();
            }
            return count;
        }
    }
}
