using Elasticsearch.Net;
using Nest;

namespace Elasticsearch.API.Extensions;

public static class ElasticsearchExtensions
{
    private const string Key = "Elastic";
    private const string V = "Url";

    public static void AddElastic(this IServiceCollection services, IConfiguration configuration)
    {
        var pool = new SingleNodeConnectionPool(new Uri(uriString: configuration.GetSection(Key)[V]));
        var settings = new ConnectionSettings(pool);
        var client = new ElasticClient(settings);
        services.AddSingleton(client);
    }
}
