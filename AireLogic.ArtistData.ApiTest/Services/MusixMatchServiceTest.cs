using System;
using Xunit;
using AireLogic.ArtistData.Services;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using FluentAssertions;

namespace AireLogic.ArtistData.ApiTest.Services
{
    public class MusixMatchServiceTest
    {
        private Mock<ILogger<MusixMatchService>> logger;
        private Mock<IConfiguration> config;
        Mock<IConfigurationSection> baseUrlConfig;
        Mock<IConfigurationSection> apiKeyConfig;
        
        const string ArtistName = "the beetles";

        public MusixMatchServiceTest()
        {
            logger = new Mock<ILogger<MusixMatchService>>();

            config = new Mock<IConfiguration>();

            baseUrlConfig = new Mock<IConfigurationSection>();
            baseUrlConfig.SetupGet(u => u.Value).Returns("https://api.musixmatch.com/ws/1.1/");
            config.Setup(c => c.GetSection("MusixMatch:BaseUrl")).Returns(baseUrlConfig.Object);

            apiKeyConfig = new Mock<IConfigurationSection>();
            apiKeyConfig.SetupGet(a => a.Value).Returns("c1ffdab49fecac0576a99d199b81fe0f");
            config.Setup(c => c.GetSection("MusixMatch:ApiKey")).Returns(apiKeyConfig.Object);
        }

        [Fact]
        public async Task TheBeetles_ShouldHaveTracks()
        {
            var response = await Target.FindArtistTracks(ArtistName);

            response.Should().ContainMatch("Strawberry Fields Forever*");
        }

        private IMusicService Target
            => new MusixMatchService(logger.Object, config.Object);
    }
}
