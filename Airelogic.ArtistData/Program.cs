using System.IO;
using System.Net.Http;
using AireLogic.ArtistData.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using ApiSeedModel = AireLogic.ApiSeed.ResponseModel;

namespace AireLogic.ArtistData
{
    class Program
    {

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            Log.Logger.Information("Application Starting");
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // TODO: allow for different services
                    services.AddTransient<IMusicService, MusixMatchService>();
                    services.AddTransient<ILyricService, ApiSeedsService>();
                    services.AddTransient<IArtistAnalyser, ArtistAnalyser>();
                })
                .UseSerilog()
                .Build();

            var startupClass = ActivatorUtilities.CreateInstance<ArtistAnalyser>(host.Services);
            startupClass.Run(args).GetAwaiter().GetResult();
        }

        static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        }
    }
}
