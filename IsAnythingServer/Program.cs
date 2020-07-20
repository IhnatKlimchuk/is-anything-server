using IsAnythingServer.Jobs;
using IsAnythingServer.Stores;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace IsAnythingServer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await PrepareHostAsync(host);
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureServices(services =>
                {
                    services.AddHostedService<JobsScheduler>();
                });

        private static async Task PrepareHostAsync(IHost host)
        {
            using var scope = host.Services.CreateScope();
            foreach (var synchronizer in scope.ServiceProvider.GetServices<IMongoDbSynchronizer>())
            {
                await synchronizer.SynchronizeAsync();
            }
        }
    }
}
