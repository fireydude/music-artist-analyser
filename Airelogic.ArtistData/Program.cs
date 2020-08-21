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

            var lyricService = new ApiSeedsService();
            foreach (var t in tracks)
            {
                Console.WriteLine($"{artistSearch} \n\t- {t}");
                var lyrics = lyricService.GetLyrics(artistSearch, t)
                    .GetAwaiter()
                    .GetResult();

                var wordCount = string.IsNullOrEmpty(lyrics) ? 0 : CountWords(lyrics);
                if (!string.IsNullOrEmpty(lyrics))
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
            var lyricService = new ApiSeedsService();
            var lyrics = lyricService.GetLyrics(artist, track)
                .GetAwaiter()
                .GetResult();

            count = string.IsNullOrEmpty(lyrics) ? 0 : CountWords(lyrics);
            return !string.IsNullOrEmpty(lyrics);
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
