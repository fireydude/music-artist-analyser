using System;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using AireLogic.ArtistData.Services;
using System.Threading.Tasks;
using FluentAssertions;

namespace AireLogic.ArtistData.Test
{
    public class ArtistAnalyserTest
    {
        private Mock<ILogger<ArtistAnalyser>> logger;
        private Mock<IMusicService> musicService;
        private Mock<ILyricService> lyricService;
        private const string ArtistName = "test";
        private readonly string[] Tracks = new string[] { "track1", "track2" };

        public ArtistAnalyserTest()
        {
            logger = new Mock<ILogger<ArtistAnalyser>>();
            musicService = new Mock<IMusicService>();
            musicService.Setup(m => m.FindArtistTracks(ArtistName)).ReturnsAsync(Tracks);
            lyricService = new Mock<ILyricService>();
            lyricService.Setup(l => l.GetLyrics(ArtistName, Tracks[0])).ReturnsAsync("this, has four words.");
            lyricService.Setup(l => l.GetLyrics(ArtistName, Tracks[1])).ReturnsAsync("this one more words at: six");
        }

        [Fact]
        public async Task EmptyArgs_LogsError()
        {
            await Target.Run(new string[] { });

            VerifyLog(LogLevel.Error, "Please provide one argument, which will be the artist name");
        }

        [Fact]
        public async Task SimpleTrackLyrics_ReturnsAverage()
        {
            var response = await Target.Run(new string[] { ArtistName });

            response.Should().Be(5);
        }

        [Fact]
        public async Task LyricLinesWithSquareBrackets_AreIgnoreBecauseTheyAreAnnotations()
        {
            lyricService.Setup(l => l.GetLyrics(ArtistName, Tracks[0])).ReturnsAsync("[ignore this line]" + System.Environment.NewLine + "this has four words");

            var response = await Target.Run(new string[] { ArtistName });

            response.Should().Be(5);
        }

        private IArtistAnalyser Target
            => new ArtistAnalyser(logger.Object, musicService.Object, lyricService.Object);

        private void VerifyLog(LogLevel level, string message)
            => logger.Verify(x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().CompareTo(message) == 0),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
    }
}
