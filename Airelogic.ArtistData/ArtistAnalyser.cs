using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AireLogic.ArtistData.Services;
using Microsoft.Extensions.Logging;

namespace AireLogic.ArtistData
{
    public class ArtistAnalyser : IArtistAnalyser
    {
        private readonly IMusicService _musicService;
        private readonly ILyricService _lyricService;
        private readonly ILogger<ArtistAnalyser> _logger;

        public ArtistAnalyser(ILogger<ArtistAnalyser> logger, IMusicService musicService, ILyricService lyricService)
        {
            _logger = logger;
            _musicService = musicService;
            _lyricService = lyricService;
        }

        public async Task<int?> Run(string[] args)
        {
            if (args.Length != 1)
            {
                _logger.LogError("Please provide one argument, which will be the artist name");
                return null;
            }
            var artistSearch = args[0];
            var allWordCounts = new List<int>();

            var tracks = await _musicService.FindArtistTracks(artistSearch);

            // TODO: use parallel tasks to get lyrics
            foreach (var t in tracks)
            {
                _logger.LogInformation("Track: {t}", t);
                var lyrics = await _lyricService.GetLyrics(artistSearch, t);

                var wordCount = string.IsNullOrEmpty(lyrics) ? 0 : CountWords(lyrics);
                if (wordCount > 0)
                {
                    allWordCounts.Add(wordCount);
                    _logger.LogInformation("Word Count: {0}", wordCount);
                }
            }

            if (tracks != null)
            {
                var averageWordCount = allWordCounts.Sum() / allWordCounts.Count();
                Console.WriteLine($"The average number of words for a song by {artistSearch} is {averageWordCount}");
                return averageWordCount;
            }
            return null;
        }

        private int CountWords(string songText)
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
