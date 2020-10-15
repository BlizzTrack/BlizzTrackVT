using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using BNetLib.Models;
using BTSharedCore.Services;
using BTVT_Worker.Workers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BTVT_Worker
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            new HostBuilder()
                .ConfigureHostConfiguration(configHost => configHost.AddEnvironmentVariables())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory())
                        .AddEnvironmentVariables()
                        .AddJsonFile("appsettings.json", true)
                        .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true);

                    if (args != null) config.AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<Mongo.DatabaseSettings>(hostContext.Configuration.GetSection(nameof(Mongo.DatabaseSettings)));
                    services.AddSingleton<Mongo>();

                    services.AddSingleton<BTSharedCore.Data.Versions>();
                    services.AddSingleton<BTSharedCore.Data.Summary>();
                    services.AddSingleton<BTSharedCore.Data.CDN>();
                    services.AddSingleton<BTSharedCore.Data.BGDL>();

                    // This lets us make adding new versions faster
                    services.AddSingleton<ConcurrentQueue<Summary>>();

                    services.AddSingleton(x => new BNetLib.Networking.BNetClient());

                    services.AddHostedService<SummaryWorker>();
                    services.AddHostedService<QueueWorker>();

                    /*
                    services.AddHostedService<VersionWorker>();
                    services.AddHostedService<CDNWorker>();
                    services.AddHostedService<BGDLWorker>();
                    */
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                });
    }
}
