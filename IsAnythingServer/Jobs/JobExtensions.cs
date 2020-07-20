using IsAnythingServer.Jobs.DailyStatistic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IsAnythingServer.Jobs
{
    public static class JobExtensions
    {
        public static IServiceCollection AddJobs(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddSingleton<IJob, DailyStatisticJob>()
                .ConfigureJobSettings<DailyStatisticJobSettings>(configuration);
        }

        public static IConfigurationSection GetJobSection<T>(this IConfiguration configuration) where T : JobSettings
        {
            return configuration.GetSection($"Jobs:{typeof(T).Name}");
        }

        public static IServiceCollection ConfigureJobSettings<T>(this IServiceCollection services, IConfiguration configuration) where T : JobSettings
        {
            return services.Configure<T>(configuration.GetJobSection<T>());
        }
    }
}
