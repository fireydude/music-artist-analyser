using AireLogic.ArtistData.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace AireLogic.ArtistData.ApiTest.Services
{
    public class ApiSeedsServiceTest
    {
        private Mock<ILogger<ApiSeedsService>> logger;
        private Mock<IConfiguration> config;
        private Mock<IConfigurationSection> baseUrlConfig;
        private Mock<IConfigurationSection> apiKeyConfig;
        
        private const string ArtistName = "The Beetles";
        private const string SongName = "Yellow Submarine";

        public ApiSeedsServiceTest()
        {
            logger = new Mock<ILogger<ApiSeedsService>>();

            config = new Mock<IConfiguration>();

            baseUrlConfig = new Mock<IConfigurationSection>();
            baseUrlConfig.SetupGet(u => u.Value).Returns("https://orion.apiseeds.com/");
            config.Setup(c => c.GetSection("ApiSeeds:BaseUrl")).Returns(baseUrlConfig.Object);

            apiKeyConfig = new Mock<IConfigurationSection>();
            apiKeyConfig.SetupGet(a => a.Value).Returns("x3ALheLaELqwzOckiIUxMw2WS5DVbxZySuKpLgefJiECPLPTTYxA2heer8pcStx2");
            config.Setup(c => c.GetSection("ApiSeeds:ApiKey")).Returns(apiKeyConfig.Object);
        }

        [Fact]
        public async Task YellowSubmarine_ShouldHaveLyrics()
        {
            var response = await Target.GetLyrics(ArtistName, SongName);

            response.Should().StartWith("In the town where I was born");
        }

        public ILyricService Target
            => new ApiSeedsService(logger.Object, config.Object);
    }
}