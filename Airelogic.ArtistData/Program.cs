using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using AireLogic.ArtistData.Services;
using ApiSeedModel = AireLogic.ApiSeed.ResponseModel;

namespace AireLogic.ArtistData
{
    class Program
    {


        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Please provide one argument, which will be the artist name");
                return;
            }
            var artistSearch = args[0];
            var allWordCounts = new List<int>();            

            IMusicService musicService = new MusixMatchService();
            var tracks = musicService.FindArtistTracks(artistSearch).GetAwaiter().GetResult();

            foreach (var t in tracks)
            {
                Console.WriteLine($"{artistSearch} \n\t- {t}");
                int wordCount;
                if (LyricWordCount(artistSearch, t, out wordCount))
                {
                    allWordCounts.Add(wordCount);
                    Console.WriteLine($"Word Count: {wordCount}\n");
                }
            }

            if (tracks != null)
            {
                var averageWordCount = allWordCounts.Sum() / allWordCounts.Count();
                Console.WriteLine($"The average number of words for a song by {artistSearch} is {averageWordCount}");
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
