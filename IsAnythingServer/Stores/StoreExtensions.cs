using IsAnythingServer.Stores.Records;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace IsAnythingServer.Stores
{
    public static class StoreExtensions
    {
        public static IServiceCollection AddStores(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddScoped<IRecordStore, MongoDbRecordStore>()
                .Configure<MongoDbSettings>(configuration.GetSection(nameof(MongoDbSettings)))
                .AddSingleton<IMongoClient, MongoClient>(serviceProvider =>
                    new MongoClient(serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value.ConnectionString))
                .AddSingleton<IMongoDatabase>(serviceProvider => serviceProvider
                    .GetRequiredService<IMongoClient>()
                    .GetDatabase(serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value.Database))
                .AddMongoCollections();
        }

        private static IServiceCollection AddMongoCollections(this IServiceCollection services)
        {
            return services
                .AddSingleton<IMongoCollection<RecordMongoDbDocument>>(serviceProvider => serviceProvider
                    .GetRequiredService<IMongoDatabase>()
                    .GetCollection<RecordMongoDbDocument>(RecordMongoDbDocument.CollectionName))
                .AddTransient<IMongoDbSynchronizer, RecordMongoDbSynchronizer>();
        }
    }
}
